using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleAim
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Controller app = new Controller();

                if (app.Init())
                {
                    app.MainLoop();
                }
                else
                {
                    Console.WriteLine("Could not initialize application.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program.Main Fatal Error: " + ex.Message);
            }
        }
    }
}
