using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Models
{
    public abstract class InvoiceItem : Model
    {
        public const string ItemRefColumnName = "ItemRef";
        public const string QuantityColumnName = "Quantity";
        public const string FeeColumnName = "Fee";
        public const string TaxColumnName = "Tax";
        public const string DiscountColumnName = "Discount";
        public const string StockRefColumnName = "StockRef";

        public int InvoiceRef { get; set; }
        public Invoice Invoice { get; set; }
        public int ItemRef { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public decimal Fee { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public int StockRef { get; set; }
        public Stock Stock { get; set; }

        public new bool Included
        {
            get
            {
                return Item != null && Invoice != null && Stock != null;
            }
        }

        public abstract Invoice.InvoiceType GetInvoiceType();
    }
}
