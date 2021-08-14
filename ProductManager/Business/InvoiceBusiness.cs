using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tappe.Data;

namespace Tappe.Business
{

    public abstract class InvoiceBusiness
    {
        protected readonly Database _database;
        
        public IEnumerable<Data.Models.Invoice> Invoices { get { return null; } }
        public IEnumerable<Data.Models.InvoiceItem> InvoiceItems { get { return null; } }

        public InvoiceBusiness()
        {
            _database = container.Create<Database>();
        }

        public abstract Data.Models.Invoice GetInvoice(int number);
        public abstract IEnumerable<Data.Models.InvoiceItem> GetInvoiceItems(int invoiceid);
        public abstract int GetLastInvoiceNumber();
        public abstract decimal GetTotalPrice();
        public abstract bool IsInvoiceNumberValid(int num);
    }
}
