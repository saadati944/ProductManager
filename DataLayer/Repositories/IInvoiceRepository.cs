using System.Data;

namespace DataLayer.Repositories
{
    interface IInvoiceRepository : IRepository
    {
        DataTable NewInvoiceDataTable();
        DataTable NewInvoiceItemDataTable();
        DataLayer.Models.Invoice GetInvoiceModel(int number);
        DataTable GetInvoice(int number);
    }
}
