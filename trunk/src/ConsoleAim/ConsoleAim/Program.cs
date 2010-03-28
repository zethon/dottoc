using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using log4net;

namespace ConsoleAim
{
    class Program
    {
        static ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                Controller app = new Controller();
                if (app.Init())
                {
                    app.MainLoop();
                }
                else
                {
                    log.Warn("Could not initialize application.");
                }
            }
            catch (Exception ex)
            {
                log.Fatal("Program.Main Fatal Error", ex);
            }
        }
    }
}
