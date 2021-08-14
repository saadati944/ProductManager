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
        public const string _tableName = "SellInvoiceItems";
        public const string _sellInvoiceColumnName = "SellInvoiceRef";
        public const string _itemRefColumnName = "ItemRef";
        public const string _quantityColumnName = "Quantity";
        public const string _feeColumnName = "Fee";
        public const string _taxColumnName = "Tax";
        public const string _discountColumnName = "Discount";

        //public int InvoiceRef { get; set; }
        //public SellInvoice Invoice { get; set; }
        //public int ItemRef { get; set; }
        //public Item Item { get; set; }
        //public int Quantity { get; set; }
        //public decimal Fee { get; set; }


        public override string[] Columns()
        {
            return new string[] { _sellInvoiceColumnName, _itemRefColumnName, _quantityColumnName, _feeColumnName, _taxColumnName, _discountColumnName};
        }

        public override string[] GetValues()
        {
            return new string[] { InvoiceRef.ToString(), ItemRef.ToString(), Quantity.ToString(), Fee.ToString(), Tax.ToString(), Discount.ToString() };
        }
        public override void MapToModel(System.Data.DataRow row)
        {
            base.MapToModel(row);
            InvoiceRef = Field(row, _sellInvoiceColumnName, InvoiceRef);
            ItemRef = Field(row, _itemRefColumnName, ItemRef);
            Quantity = Field(row, _quantityColumnName, Quantity);
            Fee = Field(row, _feeColumnName, Fee);
            Tax = Field(row, _taxColumnName, Tax);
            Discount = Field(row, _discountColumnName, Discount);
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
