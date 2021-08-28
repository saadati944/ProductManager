using Framework.Interfaces;
using Framework.DataLayer.Models;
using BuyAndSell.DataLayer.Models;
using BuyAndSell.Interfaces;
using BasicData.Interfaces;
using BasicData.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BuyAndSell.Business
{
    public class BuyInvoiceBusiness : InvoiceBusiness, IBuyInvoiceBusiness
    {
        public new IEnumerable<BuyInvoice> Invoices
        {
            get
            {
                return _database.GetAll<BuyInvoice>().ToList();
            }
        }
        public new IEnumerable<BuyInvoiceItem> InvoiceItems
        {
            get
            {
                return _database.GetAll<BuyInvoiceItem>().ToList();
            }
        }

        public BuyInvoiceBusiness(IDatabase database, ISellInvoicesRepository sellInvoicesRepository, IBuyInvoicesRepository buyInvoicesRepository, IItemsBusiness itemsBusiness) : base(database, sellInvoicesRepository, buyInvoicesRepository, itemsBusiness)
        {

        }

        public BuyInvoice FullLoadBuyInvoice(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            BuyInvoice buyInvoice = (BuyInvoice) GetInvoiceModel(number,connection, transaction);
            if (buyInvoice == null)
                return new BuyInvoice();

            else if (!buyInvoice.Included)
                buyInvoice.Include();

            buyInvoice.InvoiceItems = _buyInvoicesRepository.GetInvoiceItemsModel(buyInvoice.Id, connection, transaction);
            return buyInvoice;
        }

        public override Invoice GetInvoiceModel(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return _database.GetAll<BuyInvoice>(connection, transaction, "Number=" + number, null, 1).FirstOrDefault();
        }
        public override DataTable GetInvoice(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return _database.GetAllDataset<BuyInvoice>(connection, transaction, "Number=" + number, null, 1).Tables[0];
        }


        public override int GetLastInvoiceNumber(SqlConnection connection = null, SqlTransaction transaction = null)
        {
            try
            {
                return (int)_database.CustomeQuery(Framework.DataLayer.CustomeQueries.MaxBuyInvoiceNumber, null, null, connection, transaction).Tables[0].Rows[0][0];
            }
            catch { }
            return 0;
        }

        public override decimal GetTotalPrice()
        {
            try
            {
                return (decimal)_database.CustomeQuery(Framework.DataLayer.CustomeQueries.BuyInvoiceTotalPrice).Tables[0].Rows[0][0];
            }
            catch { }
            return 0;
        }
        public override bool EditInvoice(int lastNumber, DataTable invoicetable, DataTable invoiceitems)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);
            if (Framework.Utilities.ArrayComparator.AreEqual(GetInvoiceVersion(lastNumber, Invoice.InvoiceType.Buying, connection, transaction), invoicetable.Rows[0].Field<byte[]>(Invoice.VersionColumnName))
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
                return _database.GetAll<BuyInvoice>(null, null, "Number=" + num, null, 1).Count() == 0;
            }
            catch { }
            return false;
        }


        public byte[] GetInvoiceVersion(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return GetInvoiceVersion(number, Invoice.InvoiceType.Buying, connection, transaction);
        }
        //public int GetInvoiceVersion(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        //{
        //    return GetInvoiceVersion(number, Invoice.InvoiceType.Buying, connection, transaction);
        //}
        //public int GenerateInvoiceVersion(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        //{
        //    return GenerateInvoiceVersion(number, Invoice.InvoiceType.Buying, connection, transaction);
        //}
    }
}
