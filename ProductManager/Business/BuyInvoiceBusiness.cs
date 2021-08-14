using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Tappe.Data;
using Tappe.Data.Models;

namespace Tappe.Business
{
    public class BuyInvoiceBusiness : InvoiceBusiness
    {
        public new IEnumerable<BuyInvoice> Invoices 
        { 
            get
            {
                return _database.BuyInvoices;
            }
        }
        public new IEnumerable<BuyInvoiceItem> InvoiceItems
        {
            get
            {
                return _database.BuyInvoiceItems;
            }
        }

        public static BuyInvoice FullLoadBuyInvoice(int number)
        {
            Database db = container.Create<Database>();
            BuyInvoice buyInvoice = null;
            try
            {
                buyInvoice = db.GetAll<BuyInvoice>(null, null, "Number=" + number, null, 1).First();
            }
            catch { }
            if (buyInvoice == null)
                return new BuyInvoice();

            else if (!buyInvoice.Included)
                buyInvoice.Include();

            buyInvoice.InvoiceItems = db.GetAll<BuyInvoiceItem>(null, null, "BuyInvoiceRef=" + buyInvoice.Id);
            return buyInvoice;
        }

        public override Invoice GetInvoice(int number)
        {
            BuyInvoice bi = null;
            try
            {
                bi = _database.GetAll<BuyInvoice>(null, null, "Number=" + number, null, 1).First();
            }
            catch { }
            if (bi != null)
                return bi;
            return new BuyInvoice();
        }

        public override IEnumerable<InvoiceItem> GetInvoiceItems(int invoiceid)
        {
            return _database.GetAll<BuyInvoiceItem>(null, null, "InvoiceRef=" + invoiceid);
        }

        public override int GetLastInvoiceNumber()
        {
            try
            {
                return (int)_database.CustomeQuery(CustomeQueries.MaxBuyInvoiceNumber).Tables[0].Rows[0][0];
            }
            catch { }
            return 0;
        }

        public override decimal GetTotalPrice()
        {
            try
            {
                return (decimal) _database.CustomeQuery(CustomeQueries.BuyInvoiceTotalPrice).Tables[0].Rows[0][0];
            }
            catch { }
            return 0;
        }
        public bool SaveInvoice(BuyInvoice invoice)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);


            if (Save(invoice, connection, transaction))
            {
                _database.CommitTransaction(transaction);
                connection.Close();
                return true;
            }

            _database.RollbackTransaction(transaction);
            connection.Close();
            return false;
        }

        private bool Save(BuyInvoice invoice, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                _database.Save(invoice, connection, transaction);

                if (_database.GetAllDataset<BuyInvoice>(connection, transaction, "Number=" + invoice.Number, null, 2).Tables[0].Rows.Count != 1)
                    return false;

                var stocks = _database.GetAll<StockSummary>(connection, transaction, "StockRef=" + invoice.StockRef);

                foreach (BuyInvoiceItem x in invoice.InvoiceItems)
                {
                    x.InvoiceRef = invoice.Id;
                    _database.Save(x, connection, transaction);

                    if (x.Quantity == 0)
                        continue;

                    var q = ItemQuantity(x.ItemRef, stocks);
                    if (q == null)
                        q = new StockSummary { ItemRef = x.ItemRef, StockRef = invoice.StockRef, Quantity = 0 };
                    q.Quantity += x.Quantity;
                    _database.Save(q, connection, transaction);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        private StockSummary ItemQuantity(int itemRef, IEnumerable<StockSummary> stockSummaries)
        {
            foreach (var x in stockSummaries)
                if (itemRef == x.ItemRef)
                    return x;
            return null;
        }

        public override bool IsInvoiceNumberValid(int num)
        {
            try
            {
                return _database.GetAll<BuyInvoice>(null, null, "Number=" + num, null, 1).Count() == 0;
            }
            catch { }
            return false;
        }

    }
}
