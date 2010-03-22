using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Timers;
using System.Text.RegularExpressions;

namespace dotTOC
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class TOC
	{
        /// <summary>
        /// The first 6 bytes of every message sent from Client -> TOC and TOC -> Client
        /// </summary>
        struct flap_header
        {
            public char asterisk;
            public byte frametype;
            public short seqno;
            public short datalen;
        };

        private string _strServer = "toc.oscar.aol.com";
        public string Server
        {
            get { return _strServer; }
        }

        private int _iPort = 9898;
        public int Port
        {
            get { return _iPort; }
        }

        private Socket _socket;
        public Socket TOCSocket
        {
            get { return _socket; }
        }

        private User _user;
        public User User
        {
            get { return _user; }
        }

        public string _strClientInfo = "dotTOC - .NET TOC Library";
        public string ClientInfo
        {
            get { return _strClientInfo; }
        }

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

        #region delegates/callbacks
        public delegate void OnTOCErrorHandler(string strErrorCode);
        public event OnTOCErrorHandler OnTOCError;

		public delegate void OnDisconnectHandler();
		public event OnDisconnectHandler OnDisconnect;

        public delegate void OnDoDisconnectHandler();
        public event OnDoDisconnectHandler OnDoDisconnect;

		public delegate void OnSignedOnHandler();
		public event OnSignedOnHandler OnSignedOn;

		public delegate void OnIMInHandler(string strUser, string strMsg, bool bAuto);
		public event OnIMInHandler OnIMIn;

        public delegate void OnSendIMHander(string strUser, string strMsg, bool bAuto);
        public event OnSendIMHander OnSendIM;

		public delegate void OnUpdateBubbyHandler(Buddy buddy);
		public event OnUpdateBubbyHandler OnUpdateBuddy;

		public delegate void OnEviledHandler(int iLvl, bool bAnonymous, string strSender);
		public event OnEviledHandler OnEviled;

		public delegate void OnServerMessageHandler(string strIncoming);
		public event OnServerMessageHandler OnServerMessage;

		public delegate void OnChatJoinedHandler(string strRoomID, string strRoomName);
		public event OnChatJoinedHandler OnChatJoined;
        #endregion

        // data types received from server
		private const byte FT_SIGNON	= 1;
		private const byte FT_DATA		= 2;
		private const byte FT_ERROR		= 3;	// not used by TOC
		private const byte FT_SIGNOFF	= 4;	// not used by TOC
		private const byte FT_KEEPALIVE	= 5;

		// privates
		private bool _bDCOnPurpose = false;
		private Byte[] m_byBuff = new Byte[32767];
		private int _iSeqNum;
	
		#region contructors
		public TOC()
		{
			_user = new User();
		}

		public TOC(string strName, string strPW)
		{
            _user = new User(strName, strPW);
		}

		public TOC(string strServer, int iPort)
		{
			_strServer = strServer;
			iPort = _iPort;
		}
		#endregion constructors

		#region private_functions
		public string [] GetConfigBuddies(string strConfig)
		{	
			ArrayList names = new ArrayList();

			foreach(string strLine in strConfig.Split('\n'))
			{	
				if (strLine.ToLower().Trim() == "done:")
					break;
				
				if (strLine.StartsWith("b"))
				{
					string strTemp = strLine.Replace("\r",null);
					int i = strTemp.IndexOf(":",0);
					strTemp = strTemp.Remove(0,i+1);
					
					i = strTemp.IndexOf(":",0);
					strTemp = strTemp.Remove(i,strTemp.Length-(i-1));

					names.Add(User.Normalize(strTemp));
				}
				
			}

			return (string []) names.ToArray(typeof(string));
		}

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

            int msglen = 8 + _user.GetName().Length;
			int packetlen = 6 + msglen;

			Array.Copy(GetFlapHeader(msglen,1),packet,6);
			packet[6] = 0;
			packet[7] = 0;
			packet[8] = 0;
			packet[9] = (byte)BitConverter.ToChar(BitConverter.GetBytes(FLAP_VERSION),0);

			packet[10] = 0;
			packet[11] = (byte)BitConverter.ToChar(BitConverter.GetBytes(TLV_VERSION),0);

            packet[12] = (byte)BitConverter.ToChar(BitConverter.GetBytes(_user.GetName().Length), 1);
            packet[13] = (byte)BitConverter.ToChar(BitConverter.GetBytes(_user.GetName().Length), 0);

            Array.Copy(Encoding.ASCII.GetBytes(User.GetName()), 0, packet, 14, User.GetName().Length);
			TOCSocket.Send(packet,packetlen,0);
		}

        /// <summary>
        /// Sent immediatly after client sends FLAP SIGNONG
        /// Note: was changed from toc_signon to toc2_signon for TOC2
        /// </summary>
		private void SendUserSignOn()
		{	
			string strLogin = User.GetName();
			string strPassword = User.GetPassword(PasswordFormat.Raw);

			int code1 = (strLogin[0] - 96) * 7696 + 738816;
			int code2 = ((strPassword[0] - 96) - 1) * code1 + (strLogin[0] - 96) * 746512 + 71665152;

			string strMsg;
			strMsg = string.Format("toc2_signon {0} {1} {2} {3} {4} {5} {6} {7}",
				"login.oscar.aol.com",
				5190,
				User.GetName(),
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

		private void SetupRecieveCallback (Socket sock)
		{
		    AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
		    sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None,recieveData, sock);
		}


		public static string Encode(string strMessage)
		{
			string strRetStr = "";

			for (int i=0;i < strMessage.Length;i++)
			{
				switch(strMessage[i]) 
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

            switch (strArray[0])
            {

                case "CONFIG2":
                    //if (AutoAddBuddies)
                    //	AddBuddies(GetConfigBuddies(strIncoming));
                break;

                case "SIGN_ON":
                    Send("toc_add_buddy " + User.GetName());
                    Send("toc_set_info \"" + ClientInfo + "\"");
                    Send("toc_init_done");
                    if (OnSignedOn != null)
                    {
                        OnSignedOn();
                    }
                break;

                case "IM_IN2":
                    if (OnIMIn != null)
                    {
                        string strMsg = string.Join("", strArray, 8, strArray.Length - 8);
                        OnIMIn(User.Normalize(strArray[2]), Regex.Replace(strMsg, @"<(.|\n)*?>", string.Empty), strArray[4] == "T");
                    }
                    break;

                case "UPDATE_BUDDY2":
                    if (OnUpdateBuddy != null)
                    {
                        OnUpdateBuddy(Buddy.CreateBuddy(strIncoming));
                    }
                    break;

                case "EVILED":
                    if (OnEviled != null)
                    {
                        int iLvl = int.Parse(strArray[2]);

                        if (strArray.Length == 5)
                            OnEviled(iLvl, false, User.Normalize(strArray[4]));
                        else if (strArray.Length == 4)
                            OnEviled(iLvl, true, "");
                    }
                    break;

                case "ERROR":
                    if (OnTOCError != null)
                    {
                        // 980: invalid credentials
                        OnTOCError(strArray[2]);
                    }
                    break;

                case "CHAT_JOIN":
                    if (OnChatJoined != null)
                    {
                        OnChatJoined(strArray[2], strArray[4]);
                    }
                    break;

                case "CLIENT_EVENT2":
                    break;

                default:
                    break;
            }
        }
		#endregion private_functions

		#region public_functions

        /// <summary>
        /// Send a TOC command to the server
        /// </summary>
        /// <param name="szMsg">TOC command to be sent</param>
		public void Send(string szMsg)
		{
			const int TOC_BUFFER = 4096;

			byte [] packet = new byte[TOC_BUFFER];
			int msgLen = szMsg.Length+1;
			szMsg += (char)0;

			Array.Copy(GetFlapHeader(msgLen),packet,6);
			Array.Copy(Encoding.Default.GetBytes(szMsg),0,packet,6,msgLen);

            if (TOCSocket != null && TOCSocket.Connected)
            {
                TOCSocket.Send(packet, msgLen + 6, 0);
            }
		}

		public void SendMessage(string strUser, string strMsg)
		{
			SendMessage(strUser,strMsg,false);
		}

        /// <summary>
        /// Send an IM
        /// </summary>
        /// <param name="strUser">The destination username of the IM</param>
        /// <param name="strMsg">The message to be sent to the user</param>
        /// <param name="bAuto">A flag indicating if this is an automatic message sent by the client</param>
		public void SendMessage(string strUser,string strMsg, bool bAuto)
		{
			string strText;
			
            strText = string.Format("toc2_send_im {0} \"{1}\"{2}",
				User.Normalize(strUser),Encode(strMsg),
				bAuto ? " auto" : "");
			
            Send(strText);

            if (OnSendIM != null)
            {
                OnSendIM(strUser, strMsg, bAuto);
            }
		}

		public void Connect(string strName, string strPW)
		{
			_user = new User(strName,strPW);
			Connect();
		}

		public void Connect()
		{
            if (TOCSocket != null && TOCSocket.Connected)
            {
                Disconnect();
            }

		    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		    _socket.Blocking = false ;	
            _socket.BeginConnect(Server, Port, new AsyncCallback(OnConnect), _socket);
		}

        public void OnConnect(IAsyncResult ar)
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
			catch (Exception ex)
			{
                // TODO: rethink the try/catch handling
                //       * should it be handled in this module
                //       * what kind, if any, should?
				//DispatchError(ex.Message);
			}  
		}

		public void OnRecievedData( IAsyncResult ar )
		{
			Socket sock = (Socket)ar.AsyncState;

			try
			{
				int nBytesRead = 0;
				int nBytesRec = sock.EndReceive( ar );
				if( nBytesRec > 0 )
				{
					do 	
					{
						flap_header fh = new flap_header();
						fh.asterisk = (char)m_byBuff[nBytesRead+0];
						fh.frametype = (byte)m_byBuff[nBytesRead+1];
					
						byte [] byteTemp = new byte[2];
						byteTemp[1] = m_byBuff[nBytesRead+4];
						byteTemp[0] = m_byBuff[nBytesRead+5];
						fh.datalen = BitConverter.ToInt16(byteTemp,0);
					
						switch (fh.frametype)
						{
							case FT_SIGNON:
								SendFlapSignOn();
								SendUserSignOn();
								break;
						
							case FT_DATA:
								string sRecieved = Encoding.ASCII.GetString(m_byBuff,nBytesRead+6,fh.datalen);
								Dispatch(sRecieved);
							break;

							default:
								break;
						}		
		
						nBytesRead += fh.datalen + 6;

					} while (nBytesRead < nBytesRec);

					SetupRecieveCallback (sock);
				}
			}
			catch(Exception ex)
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

		public void AddBuddies(string [] strBuddies)
		{
			// TODO: add a 'toc2_del_group test' command to reset the group?
			string strCommand = "g:test\n";
			foreach (string strName in strBuddies)
			{
				string strTemp = strCommand + "b:"+strName+"\n";

				if (strTemp.Length >= 2048)
				{
					strCommand = "toc2_new_buddies {"+strCommand+"}";
					Send(strCommand);
					Thread.Sleep(150);
					strCommand = "toc2_new_buddies g:test\n";
				}
				else
					strCommand += "b:"+strName+"\n";
			}

			strCommand = "toc2_new_buddies {"+strCommand+"}";
			Send(strCommand);
		}

        public void SetAway()
        {
            SetAway(string.Empty);
        }

        public void SetAway(string Awaymessage)
        {
            Send(string.Format("toc_set_away \"{0}\"",Encode(Awaymessage)));
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

		#endregion public_functions
	}
}
