using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    interface IInvoiceRepository : IRepository
    {
        DataTable NewInvoiceDataTable();
        DataTable NewInvoiceItemDataTable();
        DataLayer.Models.Invoice GetInvoiceModel(int number);
        DataTable GetInvoice(int number);
    }
}
