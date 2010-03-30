using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using log4net;
using log4net.Config;

namespace WindotTOC
{
    static class Program
    {
        static ILog log = LogManager.GetLogger(typeof(Program));

        public static void AbortProgram()
        {
            log.Fatal("Aborting program");

#if (DEBUG)
            Console.ReadLine();
#endif

            Application.Exit();
        }

#if (DEBUG)
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

#if (DEBUG)
            AllocConsole();
#endif
            try
            {
                log4net.Config.XmlConfigurator.Configure();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not load log4net");
                Console.WriteLine("Error: {0}\r\n{1}", ex.Message, ex.StackTrace);
                Program.AbortProgram();
            }

            FileVersionInfo info = FileVersionInfo.GetVersionInfo("WindotTOC.exe");
            log.InfoFormat("Starting WindotTOC {0}", info.FileVersion);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());

#if (DEBUG)
            FreeConsole();
#endif
        }
    }
}
