using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public bool Init()
        {
            _commands = new Commands();
            return true;
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
    }
}
