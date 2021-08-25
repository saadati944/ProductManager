using DataLayer;
using DataLayer.Models;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Business
{

    public abstract class InvoiceBusiness
    {
        protected readonly SellInvoicesRepository _sellInvoicesRepository;
        protected readonly BuyInvoicesRepository _buyInvoicesRepository;
        protected const string _itemNameColumnName = "ItemName";
        protected const string _stockNameColumnName = "StockName";
        protected readonly Database _database;

        public IEnumerable<Invoice> Invoices { get { return null; } }
        public IEnumerable<InvoiceItem> InvoiceItems { get { return null; } }

        public InvoiceBusiness(Database database, SellInvoicesRepository sellInvoicesRepository, BuyInvoicesRepository buyInvoicesRepository)
        {
            _database = database;
            _sellInvoicesRepository = sellInvoicesRepository;
            _buyInvoicesRepository = buyInvoicesRepository;

        }

        public abstract bool EditInvoice(int lastNumber, DataTable invoicetable, DataTable invoiceitems);
        public abstract bool RemoveInvoice(int number);
        public abstract bool SaveInvoice(DataTable invoicetable, DataTable invoiceitems);
        public abstract Invoice GetInvoiceModel(int number);
        public abstract DataTable GetInvoice(int number);
        public abstract IEnumerable<InvoiceItem> GetInvoiceItemModels(int invoiceid);
        public abstract DataTable GetInvoiceItems(int invoiceid);
        public abstract int GetLastInvoiceNumber(SqlConnection connection = null, SqlTransaction transaction = null);
        public abstract decimal GetTotalPrice();
        public abstract bool IsInvoiceNumberValid(int num);


        protected StockSummary ItemQuantity(int itemRef, int stockRef, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                return _database.GetAll<StockSummary>(connection, transaction, String.Format("{0}={1} AND {2}={3}", StockSummary.StockRefColumnName, stockRef, StockSummary.ItemRefColumnName, itemRef), null, 1).First();
            }
            catch { }
            return new StockSummary { Quantity = 0, ItemRef = itemRef, StockRef = stockRef };
        }

        public static bool ValidateInvoiceDataTable(DataTable invoiceDataTable, InvoiceBusiness invoiceBusiness, int? invoiceOriginamNumber = null)
        {
            bool result = true;

            for (int i = 0; i < invoiceDataTable.Rows.Count; i++)
            {
                DataRow row = invoiceDataTable.Rows[i];
                row.ClearErrors();
                if (row[Invoice.PartyRefColumnName] is DBNull || (int)row[Invoice.PartyRefColumnName] < 1)
                {
                    row.SetColumnError(Invoice.PartyRefColumnName, "این فیلد اجباری میباشد");
                    result = false;
                }

                if (row[Invoice.UserRefColumnName] is DBNull || (int)row[Invoice.UserRefColumnName] < 1)
                {
                    row.SetColumnError(Invoice.UserRefColumnName, "این فیلد اجباری میباشد");
                    result = false;
                }

                if (row[Invoice.NumberColumnName] is DBNull || (int)row[Invoice.NumberColumnName] != (invoiceOriginamNumber == null ? -2 : invoiceOriginamNumber.Value) && !invoiceBusiness.IsInvoiceNumberValid((int)row[Invoice.NumberColumnName]))
                {
                    row.SetColumnError(Invoice.NumberColumnName, "شماره فاکتور معتبر نمیباشد");
                    result = false;
                }

                if (row[Invoice.DateColumnName] is DBNull)
                {
                    row.SetColumnError(Invoice.DateColumnName, "این فیلد اجباری میباشد");
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

        public static bool ValidateInvoiceItemsDataTable(DataTable invoiceItemsDataTable, bool checkQuantity = false)
        {
            bool result = true;

            foreach (DataRow row in invoiceItemsDataTable.Rows)
            {
                row.ClearErrors();
                bool item = true;
                if (row[InvoiceItem.ItemRefColumnName] is DBNull || (int)row[InvoiceItem.ItemRefColumnName] < 1)
                {
                    result = false;
                    if (invoiceItemsDataTable.Columns.Contains(_itemNameColumnName))
                        row.SetColumnError(_itemNameColumnName, "این فیلد اجباری میباشد");
                    else
                        row.SetColumnError(InvoiceItem.ItemRefColumnName, "این فیلد اجباری میباشد");
                    item = false;
                }

                bool stock = true;
                if (row[InvoiceItem.StockRefColumnName] is DBNull || (int)row[InvoiceItem.StockRefColumnName] < 1)
                {
                    result = false;
                    if (invoiceItemsDataTable.Columns.Contains(_stockNameColumnName))
                        row.SetColumnError(_stockNameColumnName, "این فیلد اجباری میباشد");
                    else
                        row.SetColumnError(InvoiceItem.StockRefColumnName, "این فیلد اجباری میباشد");
                    stock = false;
                }

                if (row[InvoiceItem.QuantityColumnName] is DBNull)
                {
                    result = false;
                    row.SetColumnError(InvoiceItem.QuantityColumnName, "این فیلد اجباری میباشد");
                }
                else if (checkQuantity && item && stock
                    && ItemsBusiness.GetItemQuantity((int)row[InvoiceItem.ItemRefColumnName], (int)row[InvoiceItem.StockRefColumnName]) < (int)row[InvoiceItem.QuantityColumnName])
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

        protected byte[] GetInvoiceVersion(int number, Invoice.InvoiceType invoiceType, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            if (invoiceType == Invoice.InvoiceType.Selling)
                return _database.GetAllDataset<SellInvoice>(null, null, "Number=" + number).Tables[0].Rows[0].Field<byte[]>(Invoice.VersionColumnName);
            return _database.GetAllDataset<BuyInvoice>(null, null, "Number=" + number).Tables[0].Rows[0].Field<byte[]>(Invoice.VersionColumnName);
        }
    }
}
