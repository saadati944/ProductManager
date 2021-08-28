using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Framework.Interfaces;
using Framework.DataLayer.Models;
using BuyAndSell.DataLayer.Models;
using BuyAndSell.Interfaces;
using BasicData.DataLayer.Models;

namespace BuyAndSell.Interfaces
{
    public interface IInvoiceBusiness
    {
        IEnumerable<Invoice> Invoices { get; }
        IEnumerable<InvoiceItem> InvoiceItems { get; }
        bool EditInvoice(int lastNumber, DataTable invoicetable, DataTable invoiceitems);
        bool RemoveInvoice(int number);
        bool SaveInvoice(DataTable invoicetable, DataTable invoiceitems);
        Invoice GetInvoiceModel(int number, SqlConnection connection = null, SqlTransaction transaction = null);
        DataTable GetInvoice(int number, SqlConnection connection = null, SqlTransaction transaction = null);
        int GetLastInvoiceNumber(SqlConnection connection = null, SqlTransaction transaction = null);
        decimal GetTotalPrice();
        bool IsInvoiceNumberValid(int num);
        bool ValidateInvoiceDataTable(DataTable invoiceDataTable, int? invoiceOriginamNumber = null);
        bool ValidateInvoiceItemsDataTable(DataTable invoiceItemsDataTable, bool checkQuantity = false);
    }
}
