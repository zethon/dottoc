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

        private Commands _commands;
        private dotTOC.TOC _toc;
        private string _strPrompt = string.Empty;
        private string _strLastUser = string.Empty;

        public bool Init()
        {
            _toc = new TOC();
            _toc.OnSignedOn += new TOC.OnSignedOnHandler(OnSignedOn);
            _toc.OnIMIn += new TOC.OnIMInHandler(OnIMIn);

            _commands = new Commands(this);
            return true;
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
                    Console.Write(_strPrompt + @">");
                    strInput = System.Console.ReadLine();

                    if (strInput.Length == 0)
                    {
                        continue;
                    }

                    _commands.ExecuteCommand(strInput);

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
            if (_strLastUser != string.Empty)
            {
                _toc.SendMessage(_strLastUser, strMessage);
            }
        }

        void OnIMIn(string strUser, string strMsg, bool bAuto)
        {
            _strLastUser = strUser;

            Console.WriteLine(string.Format("{0}{1}: {2}",
                strUser,
                bAuto ? " [AUTO]" : string.Empty,
                strMsg));
        }

        void OnSignedOn()
        {
            _strPrompt += "*";
            Console.WriteLine();
            Console.WriteLine("Connected...");
        }
    }
}
