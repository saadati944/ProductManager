using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tappe.Data;
using Tappe.Data.Models;

namespace Tappe.Data.Models
{
    public class SellInvoiceItem : InvoiceItem
    {
        private const string _tableName = "SellInvoiceItems";

        public const string SellInvoiceColumnName = "SellInvoiceRef";

        public override string[] Columns()
        {
            return new string[] { SellInvoiceColumnName, ItemRefColumnName, QuantityColumnName, FeeColumnName, TaxColumnName, DiscountColumnName};
        }

        public override string[] GetValues()
        {
            return new string[] { InvoiceRef.ToString(), ItemRef.ToString(), Quantity.ToString(), Fee.ToString(), Tax.ToString(), Discount.ToString() };
        }
        public override void MapToModel(System.Data.DataRow row)
        {
            base.MapToModel(row);
            InvoiceRef = GetField(row, SellInvoiceColumnName, InvoiceRef);
            ItemRef = GetField(row, ItemRefColumnName, ItemRef);
            Quantity = GetField(row, QuantityColumnName, Quantity);
            Fee = GetField(row, FeeColumnName, Fee);
            Tax = GetField(row, TaxColumnName, Tax);
            Discount = GetField(row, DiscountColumnName, Discount);
        }
        public override void Include()
        {
            Item = new Item { Id = ItemRef };
            Invoice = new SellInvoice { Id = InvoiceRef };

            Item.Load();
            Invoice.Load();
        }

        public override string TableName()
        {
            return _tableName;
        }

        public override string ToString()
        {
            if (!Included) Include();
            return String.Format("{0}_{1} * {2}", Item.Name, Quantity, Fee);
        }
        public override Invoice.InvoiceType GetInvoiceType()
        {
            return Invoice.InvoiceType.Selling;
        }
    }
}
