using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Models
{
    public class StockSummary : Model
    {
        private const string _tableName = "StockSummaries";
        private const string _itemRefColumnName = "ItemRef";
        private const string _stockRefColumnName = "StockRef";
        private const string _quantityColumnName = "Quantity";

        public int ItemRef { get; set; }
        public Item Item { get; set; }
        public int StockRef { get; set; }
        public Stock Stock { get; set; }
        public int Quantity { get; set; }

        public new bool Included
        {
            get
            {
                return Item != null && Stock != null;
            }
        }

        public override void Include()
        {
            Item = new Item { Id = ItemRef };
            Stock = new Stock { Id = StockRef };

            Item.Load();
            Stock.Load();
        }

        public override string[] Columns()
        {
            return new string[] { _itemRefColumnName, _stockRefColumnName, _quantityColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { ItemRef.ToString(), StockRef.ToString(), Quantity.ToString() };
        }

        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            ItemRef = Field(row, _itemRefColumnName, ItemRef);
            StockRef = Field(row, _stockRefColumnName, StockRef);
            Quantity = Field(row, _quantityColumnName, Quantity);
        }

        public override string TableName()
        {
            return _tableName;
        }
    }
}
