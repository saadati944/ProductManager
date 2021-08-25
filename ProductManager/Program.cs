using System;
using System.Windows.Forms;

namespace Framework
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            InstanceScanner.ScanProjects();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(Utilities.IOC.Container.GetInstance<Presentation.Forms.FrmLogin>());
        }
    }
}
