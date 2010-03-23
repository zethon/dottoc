using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotTOC;

namespace ConsoleAim
{
    class Controller
    {
        private bool _bAppQuit = false;
        public bool AppQuit
        {
            get { return _bAppQuit; }
            set { _bAppQuit = value; }
        }

        public bool BuddyNotifications
        {
            get;
            set;
        }

        private Commands _commands;
        private dotTOC.TOC _toc;
        private string _strPrompt = string.Empty;
        private string _strLastUser = string.Empty;

        private Dictionary<string, Buddy> _buddyList = new Dictionary<string, Buddy>();
        public Dictionary<string, Buddy> BuddyList
        {
            get { return _buddyList; }
        }


        private string _strCurrentUser = string.Empty;
        public string CurrentUser
        {
            get { return _strCurrentUser; }
            set { _strCurrentUser = value; }
        }

        public bool Init()
        {
            _toc = new TOC();
            _toc.OnSignedOn += new TOC.OnSignedOnHandler(OnSignedOn);
            _toc.OnIMIn += new TOC.OnIMInHandler(OnIMIn);
            _toc.OnUpdateBuddy += new TOC.OnUpdateBubbyHandler(OnUpdateBuddy);
            _toc.OnSendIM += new TOC.OnSendIMHander(OnSendIM);
            _toc.OnTOCError += new TOC.OnTOCErrorHandler(_toc_OnTOCError);

            _commands = new Commands(this);
            return true;
        }

        void _toc_OnTOCError(TOCError error)
        {
            ConsoleColor c = ConsoleColor.DarkYellow;
            Console.Write("[{0}] ", DateTime.Now.ToString("HH:mm:ss"));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Format("Error Code ({0}) `{1}`",error.Code,error.Message));

            Console.ForegroundColor = c;
        }

        public void SetAway(string strAwayText)
        {
            _toc.SetAway(strAwayText);
        }

        void OnUpdateBuddy(Buddy buddy)
        {
            if (_buddyList.ContainsKey(buddy.Name))
            {
                if (buddy.Online != _buddyList[buddy.Name].Online)
                {
                    ConsoleColor c = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("[{0}] ", DateTime.Now.ToString("HH:mm:ss"));

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(buddy.Name);

                    Console.ForegroundColor = c;
                    Console.WriteLine(" is {0}",buddy.Online ? "online" : "offline");
                }
                else if (buddy.MarkedUnavailable != _buddyList[buddy.Name].MarkedUnavailable)
                {
                    ConsoleColor c = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("[{0}] ", DateTime.Now.ToString("HH:mm:ss"));

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(buddy.Name);

                    Console.ForegroundColor = c;
                    Console.WriteLine(" is {0}", buddy.MarkedUnavailable ? "unavailable" : "available");
                }
            }
            else if (buddy.Online)
            {
                ConsoleColor c = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("[{0}] ", DateTime.Now.ToString("HH:mm:ss"));

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(buddy.Name);

                Console.ForegroundColor = c;
                Console.WriteLine(" is online");
            }

            _buddyList[buddy.Name] = buddy;
        }

        public void Login(string strUsername, string strPassword)
        {
            _toc.Connect(strUsername, strPassword);
        }

        public void MainLoop()
        {
            string strInput = string.Empty;

            try
            {
                do
                {
                    //Console.Write(_strPrompt + @">");
                    strInput = Console.ReadLine();

                    if (strInput.Length == 0)
                    {
                        continue;
                    }

                    strInput = strInput.Trim();

                    if (strInput[0] == '/' || strInput[0] == '-')
                    {
                        _commands.ExecuteCommand(strInput);
                    }
                    else
                    {
                        SendCurrent(strInput);
                    }

                    

                } while (!AppQuit);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Controller.MainLoop Exception: " + ex.Message);
            }
        }

        public void Quit()
        {
            Quit(false);
        }

        public void Quit(bool bForce)
        {
            if (bForce)
            {
                Environment.Exit(0);
            }
            else
            {
                _toc.Disconnect();
                AppQuit = true;
            }
        }

        public void Reply(string strMessage)
        {
            if (_strLastUser != string.Empty && _toc.Connected)
            {
                _toc.SendMessage(_strLastUser, strMessage);
            }
        }

        public void SendCurrent(string strMessage)
        {
            if (CurrentUser != string.Empty)
            {
                Send(CurrentUser, strMessage);
            }
            else
            {
                WriteError("No current conversation set. Use /c to set username of current conversation. Use /? for help.");
            }
        }

        public void WriteError(string strMessage)
        {
            Console.WriteLine(strMessage);
        }

        public void Send(string strUsername, string strMessage)
        {
            if (strUsername.Length < 3)
            {
                WriteError(@"Invalid username");
            }
            else
            {
                _toc.SendMessage(strUsername, strMessage);                
            }

        }

        void OnIMIn(string strUser, string strMessage, bool bAuto)
        {
            lock (this)
            {
                _strLastUser = strUser;
                ConsoleColor c = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("[{0}] ", DateTime.Now.ToString("HH:mm:ss"));

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("<");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(strUser);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("> ");

                if (bAuto)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("AUTO");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("] ");
                }

                Console.ForegroundColor = c;
                Console.WriteLine(strMessage);

            }
        }

        void OnSendIM(string strUser, string strMsg, bool bAuto)
        {
            _strLastUser = strUser;
            ConsoleColor c = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[{0}] ", DateTime.Now.ToString("HH:mm:ss"));

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("<");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(_toc.User.UserName);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("> ");

            if (bAuto)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("AUTO");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("> ");
            }

            Console.ForegroundColor = c;
            Console.WriteLine(strMsg);
        }

        void OnSignedOn()
        {
            _strPrompt += "*";
            Console.WriteLine();
            Console.WriteLine("Connected...");
        }
    }
}
