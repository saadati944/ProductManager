using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Framework.Interfaces;
using BuyAndSell.DataLayer.Models;

namespace BuyAndSell.Interfaces
{
    public interface IInvoiceRepository : IRepository
    {
        DataTable NewInvoiceDataTable();
        DataTable NewInvoiceItemDataTable();
        Invoice GetInvoiceModel(int number);
        DataTable GetInvoice(int number);
        IEnumerable<InvoiceItem> GetInvoiceItemsModel(int invoiceid, SqlConnection connection = null, SqlTransaction transaction = null);
        DataTable GetInvoiceItems(int invoiceid, SqlConnection connection = null, SqlTransaction transaction = null);
    }
}
