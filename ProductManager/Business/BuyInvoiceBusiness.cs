using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Tappe.Data;
using Tappe.Data.Models;
using System.Data;

namespace Tappe.Business
{
    public class BuyInvoiceBusiness : InvoiceBusiness
    {
        public Data.Repositories.BuyInvoicesRepository BuyInvoicesRepository
        {
            get
            {
                return _buyInvoicesRepository;
            }
        }
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
        public static BuyInvoice FullLoadBuyInvoice(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            Database db = container.Create<Database>();
            BuyInvoice buyInvoice = null;
            try
            {
                buyInvoice = db.GetAll<BuyInvoice>(connection, transaction, "Number=" + number, null, 1).First();
            }
            catch { }
            if (buyInvoice == null)
                return new BuyInvoice();

            else if (!buyInvoice.Included)
                buyInvoice.Include();

            buyInvoice.InvoiceItems = db.GetAll<BuyInvoiceItem>(connection, transaction, "BuyInvoiceRef=" + buyInvoice.Id);
            return buyInvoice;
        }

        public override Invoice GetInvoiceModel(int number)
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
        public override DataTable GetInvoice(int number)
        {
            DataTable table = _buyInvoicesRepository.NewInvoiceDataTable();

            try
            {
                DataRow row = _database.GetAllDataset<BuyInvoice>(null, null, "Number=" + number, null, 1).Tables[0].Rows[0];
                DataRow newRow = table.NewRow();
                newRow[BuyInvoice.IdColumnName] = row[BuyInvoice.IdColumnName];
                newRow[BuyInvoice.NumberColumnName] = row[BuyInvoice.NumberColumnName];
                newRow[BuyInvoice.PartyRefColumnName] = row[BuyInvoice.PartyRefColumnName];
                newRow[BuyInvoice.UserRefColumnName] = row[BuyInvoice.UserRefColumnName];
                newRow[BuyInvoice.StockRefColumnName] = row[BuyInvoice.StockRefColumnName];
                newRow[BuyInvoice.DateColumnName] = row[BuyInvoice.DateColumnName];
                newRow[BuyInvoice.TotalPriceColumnName] = row[BuyInvoice.TotalPriceColumnName];
                table.Rows.Add(newRow);
            }
            catch { }
            return table;
        }

        public override IEnumerable<InvoiceItem> GetInvoiceItemModels(int invoiceid)
        {
            return _database.GetAll<BuyInvoiceItem>(null, null, "InvoiceRef=" + invoiceid);
        }
        public override DataTable GetInvoiceItems(int invoiceid)
        {
            DataTable table = _database.GetAllDataset<BuyInvoiceItem>(null, null, "BuyInvoiceRef=" + invoiceid).Tables[0];
            return table;
        }
        public override int GetLockedInvoiceNumber()
        {
            int l = GetLastInvoiceNumber();
            while (!LockInvoiceNumber(l))
                l++;
            return l;
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
        public override bool EditInvoice(int lastNumber, DataTable invoicetable, DataTable invoiceitems)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);
            if (RemoveInvoice(lastNumber, connection, transaction) && Save(invoicetable, invoiceitems, connection, transaction))
            {
                _database.CommitTransaction(transaction);
                connection.Close();
                return true;
            }
            _database.RollbackTransaction(transaction);
            connection.Close();
            return false;
        }

        public override bool RemoveInvoice(int number)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);
            if (RemoveInvoice(number, connection, transaction))
            {
                _database.CommitTransaction(transaction);
                connection.Close();
                return true;
            }
            _database.RollbackTransaction(transaction);
            connection.Close();
            return false;
        }

        private bool RemoveInvoice(int number, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                var invoice = FullLoadBuyInvoice(number, connection, transaction);
                foreach (BuyInvoiceItem x in invoice.InvoiceItems)
                {
                    //TODO : use stock of each item
                    var q = ItemQuantity(x.ItemRef, invoice.StockRef, connection, transaction);
                    q.Quantity -= x.Quantity;
                    _database.Save(q, connection, transaction);
                    _database.Delete(x, connection, transaction);
                }
                _database.Delete(invoice, connection, transaction);
            }
            catch
            {
                return false;
            }

            return true;
        }
        public override bool SaveInvoice(DataTable invoicetable, DataTable invoiceitems)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);

            if (Save(invoicetable, invoiceitems, connection, transaction))
            {
                _database.CommitTransaction(transaction);
                connection.Close();
                return true;
            }

            _database.RollbackTransaction(transaction);
            connection.Close();
            return false;
        }
        private bool Save(DataTable invoicetable, DataTable invoiceitems, SqlConnection connection, SqlTransaction transaction)
        {
            BuyInvoice invoice = new BuyInvoice();
            invoice.MapToModel(invoicetable.Rows[0]);
            invoice.Id = -1;

            var items = new List<BuyInvoiceItem>();
            foreach (DataRow row in invoiceitems.Rows)
            {
                BuyInvoiceItem bi = new BuyInvoiceItem();
                bi.MapToModel(row);
                bi.Id = -1;
                items.Add(bi);
            }
            invoice.InvoiceItems = items;

            return Save(invoice, connection, transaction);
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
                return _database.GetAll<BuyInvoice>(null, null, "Number=" + num, null, 1).Count() == 0 && _database.GetAll<SellInvoice>(null, null, String.Format("{0}={1} AND {2}={3}", InvoiceLock.InvoiceNumberColumnName, num, InvoiceLock.InvoiceTypeColumnName, 0), null, 1).Count() == 0;
            }
            catch { }
            return false;
        }


        public bool LockInvoiceNumber(int number)
        {
            return LockInvoiceNumber(number, Invoice.InvoiceType.Buying);
        }
        public void UnlockInvoiceNumber(int number)
        {
            UnlockInvoiceNumber(number, Invoice.InvoiceType.Buying);
        }
    }
}
