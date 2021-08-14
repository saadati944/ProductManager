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

        private const string _itemRefColumnName = "ItemRef";
        private const string _priceColumnName = "Price";
        private const string _dateColumnName = "Date";

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
            return new string[] { _itemRefColumnName, _priceColumnName, _dateColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { ItemRef.ToString(), Price.ToString(), Date.ToString("yyyy-MM-dd") };
        }
        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            ItemRef = Field(row, _itemRefColumnName, ItemRef);
            Price = Field(row, _priceColumnName, Price);
            Date = Field(row, _dateColumnName, Date);
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
