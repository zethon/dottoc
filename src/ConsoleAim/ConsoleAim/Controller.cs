﻿using System;
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

            _commands = new Commands(this);
            return true;
        }

        void OnUpdateBuddy(string strUser, bool bOnline)
        {
            string strStat = @"signed on";
            if (!bOnline)
            {
                strStat = @"signed off";
            }

            Console.WriteLine(string.Format("{0} {1}", strUser, strStat), ConsoleColor.Green);            
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
                    Console.Write("> ");
                }

                Console.ForegroundColor = c;
                Console.WriteLine(strMessage);

            }
        }

        void OnSignedOn()
        {
            _strPrompt += "*";
            Console.WriteLine();
            Console.WriteLine("Connected...");
        }
    }
}