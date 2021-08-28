using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Framework.Interfaces
{
    public interface IFormFactory
    {
        T CreateForm<T>()
            where T : Form;
        void AddForm(Form frm);
    }
}
