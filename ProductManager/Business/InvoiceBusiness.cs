using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tappe.Data;
using Tappe.Data.Models;
using System.Data;
using System.Data.SqlClient;

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

        public abstract bool EditInvoice(int lastNumber, DataTable invoicetable, DataTable invoiceitems);
        public abstract bool RemoveInvoice(int number);
        public abstract bool SaveInvoice(DataTable invoicetable, DataTable invoiceitems);
        public abstract Invoice GetInvoiceModel(int number);
        public abstract DataTable GetInvoice(int number);
        public abstract IEnumerable<InvoiceItem> GetInvoiceItemModels(int invoiceid);
        public abstract DataTable GetInvoiceItems(int invoiceid);
        public abstract int GetLastInvoiceNumber();
        public abstract decimal GetTotalPrice();
        public abstract bool IsInvoiceNumberValid(int num);
        public abstract int GetLockedInvoiceNumber();


        protected StockSummary ItemQuantity(int itemRef, int stockRef, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                return container.Create<Database>().GetAll<StockSummary>(connection, transaction, "StockRef=" + stockRef + " AND ItemRef=" + itemRef, null, 1).First();
            }
            catch { }
            return new StockSummary { Quantity = 0, ItemRef = itemRef, StockRef = stockRef };
        }

        public static bool ValidateInvoiceDataTable(DataTable invoiceDataTable, InvoiceBusiness invoiceBusiness, int invoiceOriginamNumber = -1)
        {
            bool result = true;

            for (int i = 0; i < invoiceDataTable.Rows.Count; i++)
            {
                DataRow row = invoiceDataTable.Rows[i];
                if (row[Invoice.PartyRefColumnName] is DBNull || (int)row[Invoice.PartyRefColumnName] == -1)
                {
                    row.SetColumnError(Invoice.PartyRefColumnName, "این فیلد اجباری میباشد");
                    result = false;
                }

                if (row[Invoice.UserRefColumnName] is DBNull || (int)row[Invoice.UserRefColumnName] == -1)
                {
                    row.SetColumnError(Invoice.UserRefColumnName, "این فیلد اجباری میباشد");
                    result = false;
                }

                if (row[Invoice.NumberColumnName] is DBNull || (int)row[Invoice.NumberColumnName] != invoiceOriginamNumber && !invoiceBusiness.IsInvoiceNumberValid((int)row[Invoice.NumberColumnName]))
                {
                    row.SetColumnError(Invoice.NumberColumnName, "شماره فاکتور معتبر نمیباشد");
                    result = false;
                }

                if (row[Invoice.DateColumnName] is DBNull)
                {
                    row.SetColumnError(Invoice.DateColumnName, "این فیلد اجباری میباشد");
                    result = false;
                }

                if (row[Invoice.StockRefColumnName] is DBNull || (int)row[Invoice.StockRefColumnName] == -1)
                {
                    row.SetColumnError(Invoice.StockRefColumnName, "این فیلد اجباری میباشد");
                    result = false;
                }

                if (row[Invoice.TotalPriceColumnName] is DBNull)
                {
                    row.SetColumnError(Invoice.TotalPriceColumnName, "این فیلد اجباری میباشد");
                    result = false;
                }
            }

            return result;
        }

        // todo : move stock to each item
        public static bool ValidateInvoiceItemsDataTable(DataTable invoiceItemsDataTable, bool checkQuantity = false, int stockref = -1)
        {
            bool result = true;

            foreach (DataRow row in invoiceItemsDataTable.Rows)
            {
                if (row[InvoiceItem.ItemRefColumnName] is DBNull || (int)row[InvoiceItem.ItemRefColumnName] == -1)
                {
                    result = false;
                    row.SetColumnError(InvoiceItem.ItemRefColumnName, "این فیلد اجباری میباشد");
                }

                // TODO: check inventory in sell invoice
                if (row[InvoiceItem.QuantityColumnName] is DBNull)
                {
                    result = false;
                    row.SetColumnError(InvoiceItem.QuantityColumnName, "این فیلد اجباری میباشد");
                }
                else if (checkQuantity && !(row[InvoiceItem.ItemRefColumnName] is DBNull || (int)row[InvoiceItem.ItemRefColumnName] == -1)
                    && ItemsBusiness.GetItemQuantity((int)row[InvoiceItem.ItemRefColumnName], stockref) < (int)row[InvoiceItem.QuantityColumnName])
                {
                    result = false;
                    row.SetColumnError(InvoiceItem.QuantityColumnName, "موجودی ناکافی");
                }

                if (row[InvoiceItem.FeeColumnName] is DBNull)
                {
                    result = false;
                    row.SetColumnError(InvoiceItem.FeeColumnName, "این فیلد اجباری میباشد");
                }

                if (row[InvoiceItem.DiscountColumnName] is DBNull)
                {
                    result = false;
                    row.SetColumnError(InvoiceItem.DiscountColumnName, "این فیلد اجباری میباشد");
                }

                if (row[InvoiceItem.TaxColumnName] is DBNull)
                {
                    result = false;
                    row.SetColumnError(InvoiceItem.TaxColumnName, "این فیلد اجباری میباشد");
                }
            }

            return result;
        }

        protected void UnlockInvoiceNumber(int number, Invoice.InvoiceType invoiceType)
        {
            //TODO: correct this
            var locks = _database.GetAll<InvoiceLock>(null, null, String.Format("{0}={1} AND {2}={3}", InvoiceLock.InvoiceNumberColumnName, number, InvoiceLock.InvoiceTypeColumnName, invoiceType == Invoice.InvoiceType.Selling ? 1 : 0), null, 1);
            if (locks.Count() == 0)
                return;
            _database.Delete(locks.First());
        }

        protected bool LockInvoiceNumber(int number, Invoice.InvoiceType invoiceType)
        {
            var connection = _database.GetConnection();
            var transaction = _database.BeginTransaction(connection);

            if(LockInvoice(number, invoiceType, connection, transaction))
            {
                transaction.Commit();
                connection.Close();
                return true;
            }

            transaction.Rollback();
            connection.Close();
            return false;
        }
        private bool LockInvoice(int number, Invoice.InvoiceType type, SqlConnection connection, SqlTransaction transaction)
         {
            try
            {
                //TODO: correct this
                if(_database.GetAll<InvoiceLock>(connection, transaction, String.Format("{0}={1} AND {2}={3}", InvoiceLock.InvoiceNumberColumnName, number, InvoiceLock.InvoiceTypeColumnName, type == Invoice.InvoiceType.Selling ? 1:0), null, 1).Count() != 0)
                    return false;

                InvoiceLock il = new InvoiceLock { InvoiceType = type, InvoiceNumber = number };
                _database.Save(il, connection, transaction);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
