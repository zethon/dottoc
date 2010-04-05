// @author $Author$
// @version $Revision$
// @lastrevision $Date$

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Text.RegularExpressions;
using System.Reflection;

namespace dotTOC
{
    public enum FLAPTYPE
    {
		FT_SIGNON	    = 1,
		FT_DATA		    = 2,
		FT_ERROR		= 3,	// not used by TOC
		FT_SIGNOFF	    = 4,	// not used by TOC
		FT_KEEPALIVE	= 5
    }
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class TOC
	{
        private class TOCCallbackAttribute : System.Attribute
        {
            public string Callback = string.Empty;

            public TOCCallbackAttribute(string callback)
            {
                Callback = callback;
            }
        }

        public string Server = "toc.oscar.aol.com";
        public int Port = 9898;
        public readonly string ClientInfo = "dotTOC - .NET TOC Library";
        public Socket TOCSocket = null;
        
		public bool Connected
		{
            get
            {
                if (TOCSocket != null)
                {
                    return TOCSocket.Connected;
                }
                else
                {
                    return false;
                }
            }
		}

        private User _user;
        public User User
        {
            get { return _user; }
        }


        #region delegates/callbacks

        // flap type events
        public event FlapHandlers.OnFlapSignOnHandler OnFlapSignOn;
        public event FlapHandlers.OnFlapDataHandler OnFlapData;
        public event FlapHandlers.OnFlapKeepAliveHanlder OnFlapKeepAlive;
        public event FlapHandlers.OnFlapUnknownHandler OnFlapUnknown;

        // incoming events
        public event IncomingHandlers.OnServerMessageHandler OnServerMessage;
        public event IncomingHandlers.OnSignedOnHandler OnSignedOn;
        public event IncomingHandlers.OnIMInHandler OnIMIn;
        public event IncomingHandlers.OnEviledHandler OnEviled;
        public event IncomingHandlers.OnUpdateBubbyHandler OnUpdateBuddy;
        public event IncomingHandlers.OnChatJoinedHandler OnChatJoined;
        public event IncomingHandlers.OnConfigHandler OnConfig;
        public event IncomingHandlers.OnNickHandler OnNick;
        public event IncomingHandlers.OnAdminNickStatus OnAdminNickstatus;

        // outgoing events
        public event OutgoingHandlers.OnSendIMHander OnSendIM;
        public event OutgoingHandlers.OnSendServerMessageHandler OnSendServerMessage;

        public delegate void OnTOCErrorHandler(TOCError error);
        public event OnTOCErrorHandler OnTOCError;

		public delegate void OnDisconnectHandler();
		public event OnDisconnectHandler OnDisconnect;

        public delegate void OnDoDisconnectHandler();
        public event OnDoDisconnectHandler OnDoDisconnect;

        #endregion

		// privates

        /// <summary>
        /// Set to TRUE when Disconnect() is called
        /// </summary>
 		private bool _bDCOnPurpose = false;

        /// <summary>
        /// socket buffer
        /// </summary>
        private Byte[] m_byBuff = new Byte[1024 * 32]; // 32k socket buffer

        /// <summary>
        /// current buffer offset
        /// </summary>
        private int _bufferOffset = 0;

        /// <summary>
        /// sequence number for outgoing toc packets
        /// </summary>
		private int _iSeqNum;

        /// <summary>
        /// Holds temporary nick name while client waits for ADMIN_NICK_STATUS confirmation
        /// </summary>
        private string _strTempNickName = string.Empty; 

        #region back-end and login functions
        /// <summary>
        /// First message sent to server after connection
        /// </summary>
        private void SendFlapInit()
        {
            byte[] query = Encoding.Default.GetBytes("FLAPON\r\n\r\n");
            TOCSocket.Send(query);
        }

        /// <summary>
        /// Sent after FT_SIGNON recieved from server
        /// </summary>
		private void SendFlapSignOn()
		{
			const int FLAP_VERSION = 1;
			const int TLV_VERSION = 1;
			
			byte[] packet = new byte[255];

            int msglen = 8 + _user.GetNormalizeName().Length;
			int packetlen = 6 + msglen;

			Array.Copy(GetFlapHeader(msglen,1),packet,6);
			packet[6] = 0;
			packet[7] = 0;
			packet[8] = 0;
			packet[9] = (byte)BitConverter.ToChar(BitConverter.GetBytes(FLAP_VERSION),0);

			packet[10] = 0;
			packet[11] = (byte)BitConverter.ToChar(BitConverter.GetBytes(TLV_VERSION),0);

            packet[12] = (byte)BitConverter.ToChar(BitConverter.GetBytes(_user.GetNormalizeName().Length), 1);
            packet[13] = (byte)BitConverter.ToChar(BitConverter.GetBytes(_user.GetNormalizeName().Length), 0);

            Array.Copy(Encoding.ASCII.GetBytes(User.GetNormalizeName()), 0, packet, 14, User.GetNormalizeName().Length);
			TOCSocket.Send(packet,packetlen,0);
		}

        /// <summary>
        /// Sent immediatly after client sends FLAP SIGNONG
        /// Note: was changed from toc_signon to toc2_signon for TOC2
        /// </summary>
		private void SendUserSignOn()
		{
            string strLogin = User.GetNormalizeName();
			string strPassword = User.GetPassword(PasswordFormat.Raw);

			int code1 = (strLogin[0] - 96) * 7696 + 738816;
			int code2 = ((strPassword[0] - 96) - 1) * code1 + (strLogin[0] - 96) * 746512 + 71665152;

			string strMsg;
			strMsg = string.Format("toc2_signon {0} {1} {2} {3} {4} {5} {6} {7}",
				"login.oscar.aol.com",
				5190,
                User.GetNormalizeName(),
				User.GetPassword(),
				"english",
				"\"TIC:QuickBuddy\"",
				160,
				code2);

			Send(strMsg);
		}

        /// <summary>
        /// Returns the flap header send with every message the client send to the server
        /// </summary>
        /// <param name="iMsgLen">Length of message being sent.</param>
        /// <returns>The header as an array of bytes.</returns>
        private byte[] GetFlapHeader(int iMsgLen)
        {
            return GetFlapHeader(iMsgLen, 2);
        }

        /// <summary>
        /// Returns the flap header sent with every message the client sends to the server
        /// </summary>
        /// <param name="iMsgLen">Length of message being sent.</param>
        /// <param name="iFlapType">1 = flap signon, 2 = all other messages</param>
        /// <returns>The header as an array of bytes.</returns>
		private byte[] GetFlapHeader(int iMsgLen, int iFlapType)
		{
			byte [] retVal = new byte[6];
			retVal[0] = (byte)Encoding.ASCII.GetBytes("*")[0];
			retVal[1] = (byte)iFlapType;

            if (iFlapType == 1)
            {
                Random R = new Random();
                _iSeqNum = R.Next(1, 100);
            }
            else
            {
                _iSeqNum++;
            }

			retVal[2] = (byte)BitConverter.ToChar(BitConverter.GetBytes(_iSeqNum),1);
			retVal[3] = (byte)BitConverter.ToChar(BitConverter.GetBytes(_iSeqNum),0);

			retVal[4] = (byte)BitConverter.ToChar(BitConverter.GetBytes(iMsgLen),1);
			retVal[5] = (byte)BitConverter.ToChar(BitConverter.GetBytes(iMsgLen),0);

			return retVal;
		}

        /// <summary>
        /// Parses and dispataches the incoming server message to the appropriate handlers.
        /// </summary>
        /// <param name="strIncoming">Raw version of the incoming server message.</param>
        private void Dispatch(string strIncoming)
        {
            if (OnServerMessage != null)
            {
                OnServerMessage(strIncoming);
            }

            Regex r = new Regex("(:)"); // Split on colon
            string[] strArray = r.Split(strIncoming);

            foreach (MethodInfo mit in this.GetType().GetMethods())
            {
                foreach (object obj in mit.GetCustomAttributes(typeof(TOCCallbackAttribute), false))
                {
                    TOCCallbackAttribute callname = obj as TOCCallbackAttribute;
                    if (callname != null && callname.Callback.ToLower() == strArray[0].ToLower())
                    {
                        if (mit.GetParameters() != null && mit.GetParameters().Length > 0)
                        {
                            mit.Invoke(this, new object[] { strArray });
                        }
                        else
                        {
                            mit.Invoke(this, null);
                        }
                    }
                }
            }
        }

        #endregion

		#region socket functions

        public void Connect(string strName, string strPW)
        {
            _user = new User { Username = strName, DisplayName = strName, Password = strPW };
            Connect();
        }

        public bool Connect()
        {
            if (_user.GetNormalizeName() != string.Empty)
            {
                if (TOCSocket != null && TOCSocket.Connected)
                {
                    Disconnect();
                }

                TOCSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                TOCSocket.Blocking = false;
                TOCSocket.BeginConnect(Server, Port, new AsyncCallback(OnConnect), TOCSocket);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Happens when the socket connection opens
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnect(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;

            // Check if we were sucessfull
            try
            {
                if (sock.Connected)
                {
                    _bDCOnPurpose = false;
                    SendFlapInit();
                    SetupRecieveCallback(sock);
                }
                else
                {
                    // TODO: OnConnectErrorHandler?
                    //DispatchError("Connection failed.");
                }
            }
            catch// (Exception ex)
            {
                // TODO: rethink the try/catch handling
                //       * should it be handled in this module
                //       * what kind, if any, should?
                //DispatchError(ex.Message);
            }
        }

        private void SetupRecieveCallback(Socket sock)
        {
            AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
            sock.BeginReceive(m_byBuff, _bufferOffset, m_byBuff.Length - _bufferOffset, SocketFlags.None, recieveData, sock);
        }

        /// <summary>
        /// Parser of the incoming data from TOC
        /// </summary>
        /// <param name="ar"></param>
        private void OnRecievedData(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;

            try
            {
                int nBytesRead = 0;
                int nBytesRec = sock.EndReceive(ar);
                nBytesRec += _bufferOffset;

                if (nBytesRec == 0)
                    return;

                do
                {
                    FlapHeader flap = new FlapHeader(m_byBuff.Skip(nBytesRead).ToArray());

                    if (((nBytesRec - nBytesRead) + _bufferOffset) < (flap.DataLength + 6))
                    {
                        byte[] temp = m_byBuff.Skip(nBytesRead).Take(nBytesRec - nBytesRead).ToArray();
                        m_byBuff = new byte[32676];
                        Array.Copy(temp, m_byBuff, nBytesRec - nBytesRead);

                        _bufferOffset = nBytesRec - nBytesRead;
                        nBytesRead = nBytesRec + 1;
                    }
                    else
                    {
                        switch (flap.FlapType)
                        {
                            case (FLAPTYPE.FT_SIGNON):
                                if (OnFlapSignOn != null)
                                {
                                    OnFlapSignOn(flap, m_byBuff.Skip(nBytesRead).Take(flap.DataLength + 6).ToArray());
                                }
                                SendFlapSignOn();
                                SendUserSignOn();
                                break;

                            case (FLAPTYPE.FT_DATA):
                                if (OnFlapData != null)
                                {
                                    OnFlapData(flap, m_byBuff.Skip(nBytesRead).Take(flap.DataLength + 6).ToArray());
                                }

                                string sRecieved = Encoding.ASCII.GetString(m_byBuff, nBytesRead + 6, flap.DataLength);
                                Dispatch(sRecieved);
                                break;

                            case (FLAPTYPE.FT_KEEPALIVE):
                                if (OnFlapKeepAlive != null)
                                {
                                    OnFlapKeepAlive(flap, m_byBuff.Skip(nBytesRead).Take(flap.DataLength + 6).ToArray());
                                }
                                break;

                            default:
                                if (OnFlapUnknown != null)
                                {
                                    OnFlapUnknown(flap, m_byBuff.Skip(nBytesRead).Take(flap.DataLength + 6).ToArray());
                                }
                                break;
                        }

                        nBytesRead += flap.DataLength + 6;
                        _bufferOffset = 0;
                    }

                } while (nBytesRead < nBytesRec);

                SetupRecieveCallback(sock);
            }
            catch (Exception ex)
            {
                // the connection may have dropped
                if (!sock.Connected && !_bDCOnPurpose)
                {
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();

                    if (OnDisconnect != null)
                    {
                        OnDisconnect();
                    }
                }
            }
        }


        public void Disconnect()
        {
            _bDCOnPurpose = true;

            if (TOCSocket != null && TOCSocket.Connected)
            {
                TOCSocket.Shutdown(SocketShutdown.Both);
                TOCSocket.Close();

                // trigger the delegate
                if (OnDoDisconnect != null)
                {
                    OnDoDisconnect();
                }
            }
        }
        #endregion

        #region TOC Client Commands
        public static string Encode(string strMessage)
        {
            string strRetStr = "";

            for (int i = 0; i < strMessage.Length; i++)
            {
                switch (strMessage[i])
                {
                    case '$':
                    case '{':
                    case '}':
                    case '[':
                    case ']':
                    case '(':
                    case ')':
                    case '"':
                    case '\\':
                        strRetStr += '\\';
                        break;

                    default:
                        break;
                }

                strRetStr += strMessage[i];
            }

            return strRetStr;
        }

        /// <summary>
        /// Sets the user's nickname format
        /// </summary>
        /// <param name="strFormat">The format of the user's nickname</param>
        public void FormatNickname(string strFormat)
        {
            Send(string.Format("toc_format_nickname {0}", Encode(strFormat)));
            _strTempNickName = strFormat;
        }



        /// <summary>
        /// Send a TOC command to the server
        /// </summary>
        /// <param name="szMsg">TOC command to be sent</param>
        public void Send(string szMsg)
        {
            const int TOC_BUFFER = 4096;

            byte[] packet = new byte[TOC_BUFFER];
            int msgLen = szMsg.Length + 1;
            szMsg += (char)0;

            Array.Copy(GetFlapHeader(msgLen), packet, 6);
            Array.Copy(Encoding.Default.GetBytes(szMsg), 0, packet, 6, msgLen);

            if (TOCSocket != null && TOCSocket.Connected)
            {
                TOCSocket.Send(packet, msgLen + 6, 0);

                if (OnSendServerMessage != null)
                {
                    OnSendServerMessage(szMsg);
                }
            }
        }

        /// <summary>
        /// Send an IM
        /// </summary>
        /// <param name="strUser">The destination username of the IM</param>
        /// <param name="strMsg">The message to be sent to the user</param>
        /// <param name="bAuto">A flag indicating if this is an automatic message sent by the client</param>
		public void SendIM(InstantMessage im)
		{
			string strText;
			
            strText = string.Format("toc2_send_im {0} \"{1}\"{2}",
				im.To.NormalizedName.ToString(),Encode(im.Message),
				im.Auto ? " auto" : "");
			
            Send(strText);

            if (OnSendIM != null)
            {
                OnSendIM(im);
            }
		}

        /// <summary>
        /// Sets user's away status as having returned
        /// </summary>
        public void SetAvailable()
        {
            SetAway(string.Empty);
        }

        /// <summary>
        /// Set user's away status
        /// </summary>
        /// <param name="Awaymessage">Away status message</param>
        public void SetAway(string Awaymessage)
        {
            Send(string.Format("toc_set_away \"{0}\"",Encode(Awaymessage)));
        }
		#endregion public_functions

        #region TOC Server Messages

        [TOCCallback("ERROR")]
        public void DoTOCError(string[] Params)
        {
            if (OnTOCError != null)
            {
                if (Params.Length > 4)
                {
                    OnTOCError(new TOCError { Code = Params[2], Argument = Params[4] });
                }
                else
                {
                    OnTOCError(new TOCError { Code = Params[2] });
                }
            }
        }

        [TOCCallback("ADMIN_NICK_STATUS")]
        public void DoAdminNickStatus(string[] Params)
        {
            bool bSuccess = Params[2] == @"0";

            if (bSuccess)
            {
                User.DisplayName = _strTempNickName;
            }

            if (OnAdminNickstatus != null)
            {
                OnAdminNickstatus(Params[2] == @"0");
            }
        }

        [TOCCallback("CHAT_JOIN")]
        public void DoChatJoin(string[] Params)
        {
            if (OnChatJoined != null)
            {
                OnChatJoined(Params[2], Params[4]);
            }
        }

        [TOCCallback("CONFIG2")]
        public void DoConfig(string[] Params)
        {
            if (OnConfig == null)
                return;
            try
            {
                UserConfig config = new UserConfig();

                string strAll = string.Join(string.Empty, Params);
                string[] Lines = strAll.Split('\n');
                string strCurrentGroup = string.Empty;

                foreach (string strLine in Lines)
                {
                    if (strLine.ToLower().Trim() == "done:")
                    {
                        break;
                    }

                    string strTemp = strLine.Trim();
                    strTemp = strTemp.Remove(0, 2);

                    if (strTemp.IndexOf(':') != -1)
                    {
                        int iTemp = strTemp.IndexOf(':');
                        strTemp = strTemp.Remove(iTemp);
                    }

                    if (strLine.StartsWith("m"))
                    {
                        int iMode = int.Parse(strTemp);
                        config.Mode = (PermitDenyMode)Enum.ToObject(typeof(PermitDenyMode), (int)iMode);
                    }
                    else if (strLine.StartsWith("p"))
                    {
                        config.PermitList.Add(new Buddy { Name = strTemp });
                    }
                    else if (strLine.StartsWith("d"))
                    {
                        config.DenyList.Add(new Buddy { Name = strTemp });
                    }
                    else if (strLine.StartsWith("b"))
                    {
                        if (strCurrentGroup != string.Empty)
                        {
                            if (!config.BuddyList.ContainsKey(strCurrentGroup))
                            {
                                config.BuddyList.Add(strCurrentGroup, new List<Buddy>());
                            }

                            Buddy buddy = new Buddy { Name = strTemp };
                            config.BuddyList[strCurrentGroup].Add(buddy);
                        }
                    }
                    else if (strLine.StartsWith("g"))
                    {
                        if (strTemp != string.Empty)
                        {
                            strCurrentGroup = strTemp;
                        }
                    }
                }

                OnConfig(config);
            }
            catch (Exception ex)
            {
                throw new Exception(@"Invalid config sent from TOC", ex);
            }
        }

        [TOCCallback("EVILED")]
        public void DoEviled(string[] Params)
        {
            if (OnEviled != null)
            {
                int iLvl = int.Parse(Params[2]);

                if (Params.Length == 5)
                    OnEviled(iLvl, false, User.Normalize(Params[4]));
                else if (Params.Length == 4)
                    OnEviled(iLvl, true, "");
            }
        }

        [TOCCallback("IM_IN2")]
        public void DoIMIn(string[] Params)
        {
            if (OnIMIn != null)
            {
                string strMsg = string.Join("", Params, 8, Params.Length - 8);
                InstantMessage im = new InstantMessage
                {
                    From = new Buddy { Name = Params[2] },
                    RawMessage = strMsg,
                    Auto = Params[4] == "T"
                };
                OnIMIn(im);
            }
        }

        /// <summary>
        /// Callback for the TOC 'NICK' command that sends the format of the user's screen name
        /// </summary>
        /// <param name="Params"></param>
        [TOCCallback("NICK")]
        public void DoNick(string[] Params)
        {
            User.DisplayName = Params[2];

            if (OnNick != null)
                OnNick(User.DisplayName);
        }

        [TOCCallback("SIGN_ON")]
        public void DoSignOn()
        {
            Send("toc_add_buddy " + _user.GetNormalizeName());
            Send("toc_set_info \"" + ClientInfo + "\"");
            Send("toc_init_done");
            if (OnSignedOn != null)
            {
                OnSignedOn();
            }
        }

        [TOCCallback("UPDATE_BUDDY2")]
        public void DoUpdateBuddy(string[] Params)
        {
            if (OnUpdateBuddy != null)
            {
                string strIncoming = string.Join(string.Empty, Params);
                OnUpdateBuddy(Buddy.CreateBuddy(strIncoming));
            }
        }
        #endregion
    }
}
