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
    public class SellInvoiceBusiness : InvoiceBusiness, ISellInvoiceBusiness
    {
        public new IEnumerable<SellInvoice> Invoices
        {
            get
            {
                return _database.GetAll<SellInvoice>().ToList();
            }
        }
        public new IEnumerable<SellInvoiceItem> InvoiceItems
        {
            get
            {
                return _database.GetAll<SellInvoiceItem>().ToList();
            }
        }

        public SellInvoiceBusiness(IDatabase database, ISellInvoicesRepository sellInvoicesRepository, IBuyInvoicesRepository buyInvoicesRepository, IItemsBusiness itemsBusiness) : base(database, sellInvoicesRepository, buyInvoicesRepository, itemsBusiness)
        {

        }

        public SellInvoice FullLoadSellInvoice(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            SellInvoice sellInvoice = (SellInvoice)GetInvoiceModel(number, connection, transaction);
            if (sellInvoice == null)
                return new SellInvoice();

            else if (!sellInvoice.Included)
                sellInvoice.Include();

            sellInvoice.InvoiceItems = _sellInvoicesRepository.GetInvoiceItemsModel(sellInvoice.Id, connection, transaction);
            return sellInvoice;
        }

        public override Invoice GetInvoiceModel(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return _database.GetAll<SellInvoice>(connection, transaction, "Number=" + number, null, 1).FirstOrDefault();
        }
        public override DataTable GetInvoice(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return _database.GetAllDataset<SellInvoice>(connection, transaction, "Number=" + number, null, 1).Tables[0];
        }


        public override int GetLastInvoiceNumber(SqlConnection connection = null, SqlTransaction transaction = null)
        {
            try
            {
                return (int)_database.CustomeQuery(Framework.DataLayer.CustomeQueries.MaxSellInvoiceNumber, null, null, connection, transaction).Tables[0].Rows[0][0];
            }
            catch { }
            return 0;
        }

        public override decimal GetTotalPrice()
        {
            try
            {
                return (decimal)_database.CustomeQuery(Framework.DataLayer.CustomeQueries.SellInvoiceTotalPrice).Tables[0].Rows[0][0];
            }
            catch { }
            return 0;
        }
        public override DatabaseSaveResult EditInvoice(int lastNumber, DataTable invoicetable, DataTable invoiceitems)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);
            var res = RemoveInvoiceItems(lastNumber, connection, transaction) ? DatabaseSaveResult.Saved : DatabaseSaveResult.Error;
            if (res == DatabaseSaveResult.Saved) res = Save(invoicetable, invoiceitems, connection, transaction);
            if (res == DatabaseSaveResult.Saved)
                _database.CommitTransaction(transaction);
            else
                _database.RollbackTransaction(transaction);
            connection.Close();
            return res;
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
                var invoice = GetInvoiceModel(number, connection, transaction);
                if (!RemoveInvoiceItems(number, connection, transaction))
                    return false;
                _database.Delete(invoice, connection, transaction);
            }
            catch
            {
                return false;
            }

            return true;
        }
        private bool RemoveInvoiceItems(int number, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                var invoice = FullLoadSellInvoice(number, connection, transaction);
                foreach (SellInvoiceItem x in invoice.InvoiceItems)
                {
                    var q = ItemQuantity(x.ItemRef, x.StockRef, connection, transaction);
                    q.Quantity -= x.Quantity;
                    _database.Save(q, connection, transaction);
                    _database.Delete(x, connection, transaction);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }


        public override DatabaseSaveResult SaveInvoice(DataTable invoicetable, DataTable invoiceitems)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);

            var res = Save(invoicetable, invoiceitems, connection, transaction);

            if(res != DatabaseSaveResult.Saved)
                _database.RollbackTransaction(transaction);
            else
                _database.CommitTransaction(transaction);

            connection.Close();
            return res;
        }
        private DatabaseSaveResult Save(DataTable invoicetable, DataTable invoiceitems, SqlConnection connection, SqlTransaction transaction)
        {
            SellInvoice invoice = new SellInvoice();
            invoice.MapToModel(invoicetable.Rows[0]);
            
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

        private DatabaseSaveResult Save(SellInvoice invoice, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                if (invoice.UserRef < 1 || invoice.PartyRef < 1)
                    return DatabaseSaveResult.Error;

                if (invoice.Number == -1)
                    invoice.Number = GetLastInvoiceNumber(connection, transaction) + 1;

                if (_database.Save(invoice, connection, transaction) == DatabaseSaveResult.AlreadyChanged)
                    return DatabaseSaveResult.AlreadyChanged;

                if (_database.GetAllDataset<SellInvoice>(connection, transaction, "Number=" + invoice.Number, null, 2).Tables[0].Rows.Count != 1)
                    return DatabaseSaveResult.Error;

                foreach (SellInvoiceItem x in invoice.InvoiceItems)
                {
                    if (x.ItemRef < 1 || x.StockRef < 1)
                        return DatabaseSaveResult.Error;

                    x.InvoiceRef = invoice.Id;
                    _database.Save(x, connection, transaction);

                    if (x.Quantity == 0)
                        continue;

                    var q = ItemQuantity(x.ItemRef, x.StockRef, connection, transaction);

                    if (q == null || q.Quantity < x.Quantity)
                        return DatabaseSaveResult.Error;
                    q.Quantity -= x.Quantity;

                    _database.Save(q, connection, transaction);
                }

            }
            catch
            {
                return DatabaseSaveResult.Error;
            }

            return DatabaseSaveResult.Saved;
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
