namespace DataLayer.Models
{
    public class SellInvoice : Invoice
    {
        public override InvoiceType GetInvoiceType()
        {
            return InvoiceType.Selling;
        }
    }
}
