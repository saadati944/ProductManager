using DataLayer;
using DataLayer.Models;
using DataLayer.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utilities;

namespace Business
{
    public class SellInvoiceBusiness : InvoiceBusiness
    {
        public SellInvoicesRepository SellInvoicesRepository
        {
            get
            {
                return _sellInvoicesRepository;
            }
        }
        public new IEnumerable<SellInvoice> Invoices
        {
            get
            {
                return _database.SellInvoices;
            }
        }
        public new IEnumerable<SellInvoiceItem> InvoiceItems
        {
            get
            {
                return _database.SellInvoiceItems;
            }
        }

        public SellInvoiceBusiness(Database database, SellInvoicesRepository sellInvoicesRepository, BuyInvoicesRepository buyInvoicesRepository) : base(database, sellInvoicesRepository, buyInvoicesRepository)
        {

        }

        public static SellInvoice FullLoadSellInvoice(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            Database db = IOC.Container.GetInstance<Database>();
            SellInvoice sellInvoice = null;
            try
            {
                sellInvoice = db.GetAll<SellInvoice>(connection, transaction, "number=" + number, null, 1).First();
            }
            catch { }
            if (sellInvoice == null)
                return new SellInvoice();

            else if (!sellInvoice.Included)
                sellInvoice.Include();

            sellInvoice.InvoiceItems = db.GetAll<SellInvoiceItem>(connection, transaction, "SellInvoiceRef=" + sellInvoice.Id);
            return sellInvoice;
        }

        public override Invoice GetInvoiceModel(int number)
        {
            //TODO: remove this function
            return _sellInvoicesRepository.GetInvoiceModel(number);
        }

        public override DataTable GetInvoice(int number)
        {
            //TODO: remove this function
            return _sellInvoicesRepository.GetInvoice(number);
        }

        public override IEnumerable<InvoiceItem> GetInvoiceItemModels(int invoiceid)
        {
            return _database.GetAll<SellInvoiceItem>(null, null, "InvoiceRef=" + invoiceid);
        }
        public override DataTable GetInvoiceItems(int invoiceid)
        {
            DataTable table = _database.GetAllDataset<SellInvoiceItem>(null, null, "SellInvoiceRef=" + invoiceid).Tables[0];
            return table;
        }

        public override int GetLastInvoiceNumber(SqlConnection connection = null, SqlTransaction transaction = null)
        {
            try
            {
                return (int)_database.CustomeQuery(CustomeQueries.MaxSellInvoiceNumber, null, null, connection, transaction).Tables[0].Rows[0][0];
            }
            catch { }
            return 0;
        }

        public override decimal GetTotalPrice()
        {
            try
            {
                return (decimal)_database.CustomeQuery(CustomeQueries.SellInvoiceTotalPrice).Tables[0].Rows[0][0];
            }
            catch { }
            return 0;
        }
        public override bool EditInvoice(int lastNumber, DataTable invoicetable, DataTable invoiceitems)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);

            if (ArrayComparator.AreEqual(GetInvoiceVersion(lastNumber, Invoice.InvoiceType.Selling, connection, transaction), invoicetable.Rows[0].Field<byte[]>(Invoice.VersionColumnName))
                && RemoveInvoice(lastNumber, connection, transaction) && Save(invoicetable, invoiceitems, connection, transaction))
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
                var invoice = FullLoadSellInvoice(number, connection, transaction);
                foreach (SellInvoiceItem x in invoice.InvoiceItems)
                {
                    var q = ItemQuantity(x.ItemRef, x.StockRef, connection, transaction);
                    q.Quantity += x.Quantity;
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
            SellInvoice invoice = new SellInvoice();
            invoice.MapToModel(invoicetable.Rows[0]);
            invoice.Id = -1;

            var items = new List<SellInvoiceItem>();
            foreach (DataRow row in invoiceitems.Rows)
            {
                SellInvoiceItem bi = new SellInvoiceItem();
                bi.MapToModel(row);
                bi.Id = -1;
                items.Add(bi);
            }
            invoice.InvoiceItems = items;

            return Save(invoice, connection, transaction);
        }

        private bool Save(SellInvoice invoice, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                if (invoice.UserRef < 1 || invoice.PartyRef < 1)
                    return false;

                if (invoice.Number == -1)
                    invoice.Number = GetLastInvoiceNumber(connection, transaction) + 1;

                _database.Save(invoice, connection, transaction);

                if (_database.GetAllDataset<SellInvoice>(connection, transaction, "Number=" + invoice.Number, null, 2).Tables[0].Rows.Count != 1)
                    return false;

                foreach (SellInvoiceItem x in invoice.InvoiceItems)
                {
                    if (x.ItemRef < 1 || x.StockRef < 1)
                        return false;

                    x.InvoiceRef = invoice.Id;
                    _database.Save(x, connection, transaction);

                    if (x.Quantity == 0)
                        continue;

                    var q = ItemQuantity(x.ItemRef, x.StockRef, connection, transaction);

                    if (q == null || q.Quantity < x.Quantity)
                        return false;
                    q.Quantity -= x.Quantity;

                    _database.Save(q, connection, transaction);
                }

            }
            catch
            {
                return false;
            }

            return true;
        }

        public override bool IsInvoiceNumberValid(int num)
        {
            try
            {
                return _database.GetAll<SellInvoice>(null, null, "Number=" + num, null, 1).Count() == 0;
            }
            catch { }
            return false;
        }


        public byte[] GetInvoiceVersion(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return GetInvoiceVersion(number, Invoice.InvoiceType.Selling, connection, transaction);
        }
    }
}
