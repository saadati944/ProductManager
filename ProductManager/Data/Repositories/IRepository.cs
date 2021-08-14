using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Tappe.Data.Repositories
{
    public delegate void DataChangeHandler();
    public interface IRepository
    {
        DataTable DataTable { get; }


        event DataChangeHandler DataChanged;
        void Update();
    }
}
