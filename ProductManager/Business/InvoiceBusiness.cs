using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tappe.Data;
using Tappe.Data.Models;
using System.Data;

namespace Tappe.Business
{

    public abstract class InvoiceBusiness
    {
        protected readonly Data.Repositories.SellInvoicesRepository _sellInvoicesRepository;
        protected readonly Data.Repositories.BuyInvoicesRepository _buyInvoicesRepository;
        protected readonly Database _database;
        
        public IEnumerable<Invoice> Invoices { get { return null; } }
        public IEnumerable<InvoiceItem> InvoiceItems { get { return null; } }

        public InvoiceBusiness()
        {
            _database = container.Create<Database>();
            _sellInvoicesRepository = container.Create<Data.Repositories.SellInvoicesRepository>();
            _buyInvoicesRepository = container.Create<Data.Repositories.BuyInvoicesRepository>();
        }

        public abstract Invoice GetInvoiceModel(int number);
        public abstract DataTable GetInvoice(int number);
        public abstract IEnumerable<InvoiceItem> GetInvoiceItems(int invoiceid);
        public abstract int GetLastInvoiceNumber();
        public abstract decimal GetTotalPrice();
        public abstract bool IsInvoiceNumberValid(int num);
    }
}
