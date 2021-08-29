using BasicData.DataLayer.Models;
using System;

namespace BuyAndSell.DataLayer.Models
{
    public class SellInvoiceItem : InvoiceItem
    {
        private const string _tableName = "SellInvoiceItems";

        public const string SellInvoiceColumnName = "SellInvoiceRef";

        public override string[] Columns()
        {
            return new string[] { SellInvoiceColumnName, ItemRefColumnName, QuantityColumnName, FeeColumnName, TaxColumnName, DiscountColumnName, StockRefColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { InvoiceRef.ToString(), ItemRef.ToString(), Quantity.ToString(), Fee.ToString(), Tax.ToString(), Discount.ToString(), StockRef.ToString() };
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
            StockRef = GetField(row, StockRefColumnName, StockRef);
        }
        public override void Include()
        {
            Item = new Item { Id = ItemRef };
            Invoice = new SellInvoice { Id = InvoiceRef };
            Stock = new Stock { Id = StockRef };

            Load(Item);
            Load(Invoice);
            Load(Stock);
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
