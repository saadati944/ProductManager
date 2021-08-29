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
    public abstract class InvoiceBusiness : IInvoiceBusiness
    {
        protected readonly ISellInvoicesRepository _sellInvoicesRepository;
        protected readonly IBuyInvoicesRepository _buyInvoicesRepository;
        protected readonly IItemsBusiness _itemsBusiness;
        protected const string _itemNameColumnName = "ItemName";
        protected const string _stockNameColumnName = "StockName";
        protected readonly IDatabase _database;

        public IEnumerable<Invoice> Invoices { get { return null; } }
        public IEnumerable<InvoiceItem> InvoiceItems { get { return null; } }

        public InvoiceBusiness(IDatabase database, ISellInvoicesRepository sellInvoicesRepository, IBuyInvoicesRepository buyInvoicesRepository, IItemsBusiness itemsBusiness)
        {
            _database = database;
            _sellInvoicesRepository = sellInvoicesRepository;
            _buyInvoicesRepository = buyInvoicesRepository;
            _itemsBusiness = itemsBusiness;
        }

        public abstract DatabaseSaveResult EditInvoice(int lastNumber, DataTable invoicetable, DataTable invoiceitems);
        public abstract bool RemoveInvoice(int number);
        public abstract DatabaseSaveResult SaveInvoice(DataTable invoicetable, DataTable invoiceitems);
        public abstract Invoice GetInvoiceModel(int number, SqlConnection connection = null, SqlTransaction transaction = null);
        public abstract DataTable GetInvoice(int number, SqlConnection connection = null, SqlTransaction transaction = null);
        public abstract int GetLastInvoiceNumber(SqlConnection connection = null, SqlTransaction transaction = null);
        public abstract decimal GetTotalPrice();
        public abstract bool IsInvoiceNumberValid(int num);


        protected StockSummary ItemQuantity(int itemRef, int stockRef, SqlConnection connection, SqlTransaction transaction)
        {
            var ss = _database.GetAll<StockSummary>(connection, transaction, String.Format("{0}={1} AND {2}={3}", StockSummary.StockRefColumnName, stockRef, StockSummary.ItemRefColumnName, itemRef), null, 1).FirstOrDefault();
            if(ss == null)
                ss = new StockSummary { Quantity = 0, ItemRef = itemRef, StockRef = stockRef };
            return ss;
        }

        public bool ValidateInvoiceDataTable(DataTable invoiceDataTable, int? invoiceOriginamNumber = null)
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

                if (row[Invoice.NumberColumnName] is DBNull || (int)row[Invoice.NumberColumnName] != (invoiceOriginamNumber == null ? -2 : invoiceOriginamNumber.Value) && !IsInvoiceNumberValid((int)row[Invoice.NumberColumnName]))
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

        public bool ValidateInvoiceItemsDataTable(DataTable invoiceItemsDataTable, bool checkQuantity = false)
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
                    && _itemsBusiness.GetItemQuantity((int)row[InvoiceItem.ItemRefColumnName], (int)row[InvoiceItem.StockRefColumnName]) < (int)row[InvoiceItem.QuantityColumnName])
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
                return _database.GetAllDataset<SellInvoice>(connection, transaction, "Number=" + number).Tables[0].Rows[0].Field<byte[]>(Invoice.VersionColumnName);
            return _database.GetAllDataset<BuyInvoice>(connection, transaction, "Number=" + number).Tables[0].Rows[0].Field<byte[]>(Invoice.VersionColumnName);
        }
    }
}
