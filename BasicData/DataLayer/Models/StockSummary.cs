using System.Data;
using Framework.DataLayer.Models;

namespace BasicData.DataLayer.Models
{
    public class StockSummary : Model
    {
        private const string _tableName = "StockSummaries";

        public const string ItemRefColumnName = "ItemRef";
        public const string StockRefColumnName = "StockRef";
        public const string QuantityColumnName = "Quantity";

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

            Load(Item);
            Load(Stock);
        }

        public override string[] Columns()
        {
            return new string[] { ItemRefColumnName, StockRefColumnName, QuantityColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { ItemRef.ToString(), StockRef.ToString(), Quantity.ToString() };
        }

        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            ItemRef = GetField(row, ItemRefColumnName, ItemRef);
            StockRef = GetField(row, StockRefColumnName, StockRef);
            Quantity = GetField(row, QuantityColumnName, Quantity);
        }

        public override string TableName()
        {
            return _tableName;
        }
    }
}
