using Tappe.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tappe
{
    static class Program
    {
        public static StructureMap.IContainer Container;
        public static Data.Models.User LoggedInUser;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Container = StructureMap.Container.For<InstanceScanner>();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Forms.FrmLogin());
        }
    }
}
