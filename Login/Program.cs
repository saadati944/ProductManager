using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Login
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Framework.InstanceScanner.ScanProjects();
            new LoginRegistration();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var formFactory = Framework.Utilities.IOC.Container.GetInstance<Framework.Utilities.FormFactory>();
            Application.Run(formFactory.CreateForm<Presentation.Forms.FrmLogin>());
        }
    }
}
