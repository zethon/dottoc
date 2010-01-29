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

        public bool Init()
        {
            _toc = new TOC();
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

            do
            {
                Console.Write(@">");
                strInput = System.Console.ReadLine();

                if (strInput.Length == 0)
                {
                    continue;
                }

                _commands.ExecuteCommand(strInput);

            } while (!AppQuit);
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
                AppQuit = true;
            }
        }
    }
}
