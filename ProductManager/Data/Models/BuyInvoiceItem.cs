using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Models
{
    public class BuyInvoiceItem : InvoiceItem
    {
        private const string _tableName = "BuyInvoiceItems";
        private const string _buyInvoiceRefColumnName = "BuyInvoiceRef";
        private const string _itemRefColumnName = "ItemRef";
        private const string _quantityColumnName = "Quantity";
        private const string _feeColumnName = "Fee";
        private const string _taxColumnName = "Tax";
        private const string _discountColumnName = "Discount";

        //public int InvoiceRef { get; set; }
        //public Invoice Invoice { get; set; }
        //public int ItemRef { get; set; }
        //public Item Item { get; set; }
        //public int Quantity { get; set; }
        //public decimal Fee { get; set; }


        public override string[] Columns()
        {
            return new string[] { _buyInvoiceRefColumnName, _itemRefColumnName, _quantityColumnName, _feeColumnName, _taxColumnName, _discountColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { InvoiceRef.ToString(), ItemRef.ToString(), Quantity.ToString(), Fee.ToString(), Tax.ToString(), Discount.ToString() };
        }
        public override void MapToModel(System.Data.DataRow row)
        {
            base.MapToModel(row);
            InvoiceRef = GetField(row, _buyInvoiceRefColumnName, InvoiceRef);
            ItemRef = GetField(row, _itemRefColumnName, ItemRef);
            Quantity = GetField(row, _quantityColumnName, Quantity);
            Fee = GetField(row, _feeColumnName, Fee);
            Tax = GetField(row, _taxColumnName, Tax);
            Discount = GetField(row, _discountColumnName, Discount);
        }
        public override void Include()
        {
            Item = new Item { Id = ItemRef };
            Invoice = new BuyInvoice { Id = InvoiceRef };

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
            return Invoice.InvoiceType.Buying;
        }
    }
}
