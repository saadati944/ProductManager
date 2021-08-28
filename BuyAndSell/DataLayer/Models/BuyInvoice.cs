
namespace BuyAndSell.DataLayer.Models
{
    public class BuyInvoice : Invoice
    {
        public override InvoiceType GetInvoiceType()
        {
            return InvoiceType.Buying;
        }
    }
}
