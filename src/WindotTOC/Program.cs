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
            log4net.Config.XmlConfigurator.Configure();

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
