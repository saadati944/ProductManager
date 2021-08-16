using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Tappe.Data;
using Tappe.Data.Models;

namespace Tappe.Business
{
    public class SellInvoiceBusiness : InvoiceBusiness
    {
        public Data.Repositories.SellInvoicesRepository SellInvoicesRepository
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

        public static SellInvoice FullLoadSellInvoice(int number, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            Database db = container.Create<Database>();
            SellInvoice sellInvoice = null;
            try
            {
                sellInvoice = db.GetAll<SellInvoice>(null, null, "number=" + number, null, 1).First();
            }
            catch { }
            if (sellInvoice == null)
                return new SellInvoice();

            else if (!sellInvoice.Included)
                sellInvoice.Include();

            sellInvoice.InvoiceItems = db.GetAll<SellInvoiceItem>(null, null, "SellInvoiceRef=" + sellInvoice.Id);
            return sellInvoice;
        }

        public override Invoice GetInvoiceModel(int number)
        {
            SellInvoice si = null;
            try
            {
                si = _database.GetAll<SellInvoice>(null, null, "Number=" + number, null, 1).First();
            }
            catch { }
            if (si != null)
                return si;
            return new SellInvoice();
        }

        public override DataTable GetInvoice(int number)
        {
            DataTable table = _buyInvoicesRepository.NewInvoiceDataTable();

            try
            {
                DataRow row = _database.GetAllDataset<SellInvoice>(null, null, "Number=" + number, null, 1).Tables[0].Rows[0];
                DataRow newRow = table.NewRow();
                newRow[SellInvoice.IdColumnName] = row[SellInvoice.IdColumnName];
                newRow[SellInvoice.NumberColumnName] = row[SellInvoice.NumberColumnName];
                newRow[SellInvoice.PartyRefColumnName] = row[SellInvoice.PartyRefColumnName];
                newRow[SellInvoice.UserRefColumnName] = row[SellInvoice.UserRefColumnName];
                newRow[SellInvoice.StockRefColumnName] = row[SellInvoice.StockRefColumnName];
                newRow[SellInvoice.DateColumnName] = row[SellInvoice.DateColumnName];
                newRow[SellInvoice.TotalPriceColumnName] = row[SellInvoice.TotalPriceColumnName];
                table.Rows.Add(newRow);
            }
            catch { }
            return table;
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


        public override int GetLastInvoiceNumber()
        {
            try
            {
                return (int)_database.CustomeQuery(CustomeQueries.MaxSellInvoiceNumber).Tables[0].Rows[0][0];
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

        public override bool RemoveInvoice(int number)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);
            if(RemoveInvoice(number, connection, transaction))
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
                foreach(SellInvoiceItem x in invoice.InvoiceItems)
                {
                    //TODO : use stock of each item
                    var q = ItemQuantity(x.ItemRef, invoice.StockRef, connection, transaction);
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
            SellInvoice invoice = new SellInvoice();
            invoice.MapToModel(invoicetable.Rows[0]);
            var items = new List<SellInvoiceItem>();
            foreach (DataRow row in invoiceitems.Rows)
            {
                SellInvoiceItem si = new SellInvoiceItem();
                si.MapToModel(row);
                items.Add(si);
            }
            invoice.InvoiceItems = items;
            return SaveInvoice(invoice);
        }

        public bool SaveInvoice(SellInvoice invoice)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);


            if(Save(invoice, connection, transaction))
            {
                _database.CommitTransaction(transaction);
                connection.Close();
                return true;
            }

            _database.RollbackTransaction(transaction);
            connection.Close();
            return false;
        }

        private bool Save(SellInvoice invoice, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                _database.Save(invoice, connection, transaction);

                if (_database.GetAllDataset<SellInvoice>(connection, transaction, "Number=" + invoice.Number, null, 2).Tables[0].Rows.Count != 1)
                    return false;

                var stocks = _database.GetAll<StockSummary>(connection, transaction, "StockRef=" + invoice.StockRef);

                foreach (SellInvoiceItem x in invoice.InvoiceItems)
                {
                    x.InvoiceRef = invoice.Id;
                    _database.Save(x, connection, transaction);

                    if (x.Quantity == 0)
                        continue;

                    var q = ItemQuantity(x.ItemRef, stocks);

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
                return _database.GetAll<SellInvoice>(null, null, "Number=" + num, null, 1).Count() == 0;
            }
            catch { }
            return false;
        }


    }
}
