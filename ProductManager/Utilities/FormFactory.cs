using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    public class FormFactory : Interfaces.IFormFactory
    {
        public T CreateForm<T>()
            where T : Form
        {
            return IOC.Container.GetInstance<T>();
        }
        public void AddForm(Form frm)
        {
            CallOnFormAdded(frm);
        }

        public delegate void FormAddedEventHandler(Form frm);
        public static event FormAddedEventHandler OnFormAdded;
        private static void CallOnFormAdded(Form frm)
        {
            if (OnFormAdded != null)
                OnFormAdded(frm);
        }
    }
}
