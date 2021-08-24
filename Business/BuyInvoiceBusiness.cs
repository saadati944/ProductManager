using DataLayer;
using DataLayer.Repositories;
using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utilities;

namespace Business
{
    public class BuyInvoiceBusiness : InvoiceBusiness
    {
        public BuyInvoicesRepository BuyInvoicesRepository
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

        public BuyInvoiceBusiness(Database database, SellInvoicesRepository sellInvoicesRepository, BuyInvoicesRepository buyInvoicesRepository) : base(database, sellInvoicesRepository, buyInvoicesRepository)
        {

        }

        public static BuyInvoice FullLoadBuyInvoice(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            Database db = IOC.Container.GetInstance<Database>();
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
            //TODO: remove this function
            return _buyInvoicesRepository.GetInvoiceModel(number);
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
            //TODO: remove this function
            return _buyInvoicesRepository.GetInvoice(number);
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
            int l = GetLastInvoiceNumber() + 1;
            while (!LockInvoiceNumber(l))
                l++;
            return l;
        }
        public override int GetLastInvoiceNumber(SqlConnection connection = null, SqlTransaction transaction = null)
        {
            try
            {
                return (int)_database.CustomeQuery(CustomeQueries.MaxBuyInvoiceNumber, null, null, connection, transaction).Tables[0].Rows[0][0];
            }
            catch { }
            return 0;
        }

        public override decimal GetTotalPrice()
        {
            try
            {
                return (decimal)_database.CustomeQuery(CustomeQueries.BuyInvoiceTotalPrice).Tables[0].Rows[0][0];
            }
            catch { }
            return 0;
        }
        public override bool EditInvoice(int lastNumber, int version, DataTable invoicetable, DataTable invoiceitems)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);
            if (GetInvoiceVersion(lastNumber, connection, transaction) == version && RemoveInvoice(lastNumber, connection, transaction) && Save(invoicetable, invoiceitems, connection, transaction))
            {
                GenerateInvoiceVersion(lastNumber, connection, transaction);
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
                    var q = ItemQuantity(x.ItemRef, x.StockRef, connection, transaction);
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
                if (invoice.UserRef < 1 || invoice.PartyRef < 1)
                    return false;

                if (invoice.Number == -1)
                    invoice.Number = GetLastInvoiceNumber(connection, transaction) + 1;

                _database.Save(invoice, connection, transaction);

                if (_database.GetAllDataset<BuyInvoice>(connection, transaction, "Number=" + invoice.Number, null, 2).Tables[0].Rows.Count != 1)
                    return false;

                foreach (BuyInvoiceItem x in invoice.InvoiceItems)
                {
                    if (x.ItemRef < 1 || x.StockRef < 1)
                        return false;
                    x.InvoiceRef = invoice.Id;
                    _database.Save(x, connection, transaction);

                    if (x.Quantity == 0)
                        continue;

                    var q = ItemQuantity(x.ItemRef, x.StockRef, connection, transaction);
                    if (q == null)
                        q = new StockSummary { ItemRef = x.ItemRef, StockRef = x.StockRef, Quantity = 0 };
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

        public override bool IsInvoiceNumberValid(int num)
        {
            try
            {
                return _database.GetAll<BuyInvoice>(null, null, "Number=" + num, null, 1).Count() == 0 && _database.GetAll<InvoiceLock>(null, null, String.Format("{0}={1} AND {2}={3}", InvoiceLock.InvoiceNumberColumnName, num, InvoiceLock.InvoiceTypeColumnName, 0), null, 1).Count() == 0;
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

        public int GetInvoiceVersion(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return GetInvoiceVersion(number, Invoice.InvoiceType.Buying, connection, transaction);
        }
        public int GenerateInvoiceVersion(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return GenerateInvoiceVersion(number, Invoice.InvoiceType.Buying, connection, transaction);
        }
    }
}
