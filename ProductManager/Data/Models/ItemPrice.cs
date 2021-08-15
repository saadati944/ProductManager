using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Models
{
    public class ItemPrice : Model
    {
        private const string _tableName = "ItemPrices";

        public const string ItemRefColumnName = "ItemRef";
        public const string PriceColumnName = "Price";
        public const string DateColumnName = "Date";

        public int ItemRef { get; set; }
        public Item Item { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public new bool Included
        {
            get
            {
                return Item != null;
            }
        }
        public override void Include()
        {
            Item = new Item { Id = ItemRef };
            
            Item.Load();
        }

        public override string[] Columns()
        {
            return new string[] { ItemRefColumnName, PriceColumnName, DateColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { ItemRef.ToString(), Price.ToString(), Date.ToString("yyyy-MM-dd") };
        }
        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            ItemRef = GetField(row, ItemRefColumnName, ItemRef);
            Price = GetField(row, PriceColumnName, Price);
            Date = GetField(row, DateColumnName, Date);
        }

        public override string TableName()
        {
            return _tableName;
        }

        public override string ToString()
        {
            if (!Included)
                Include();
            return string.Format("{0} _ {1} _ {2}", Item.Name, PersianDate.PersianDateStringFromDateTime(Date), Price);
        }
    }
}
