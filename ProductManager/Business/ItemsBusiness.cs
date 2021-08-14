using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tappe.Data;
using System.Data;

namespace Tappe.Business
{
    public class ItemsBusiness
    {
        private readonly Database _database;
        private readonly Data.Repositories.ItemsRepository _itemsRepository;

        private const string _nameColumnName = "Name";
        private const string _idColumnName = "Id";

        public Data.Repositories.ItemsRepository ItemsRepository
        {
            get
            {
                return _itemsRepository;
            }
        }

        public IEnumerable<Data.Models.Item> Items
        {
            get
            {
                return _database.Items;
            }
        }

        public ItemsBusiness()
        {
            _database = container.Create<Database>();
            _itemsRepository = container.Create<Data.Repositories.ItemsRepository>();
        }
        public bool IsItemNameExists(string name)
        {
            return _database.GetAll<Data.Models.Item>(null, null, "Name='" + name.Replace("'", "''") + "'", null, 1).Count() != 0;
        }

        public static int GetItemQuantity(int itemRef, int stockRef)
        {
            try
            {
                return container.Create<Database>().GetAll<Data.Models.StockSummary>(null, null, "StockRef="+stockRef+" AND ItemRef=" + itemRef, null, 1).First().Quantity;
            }
            catch { }
            return 0;
        }

        public Data.Models.Item GetItem(string name)
        {
            foreach (DataRow x in _itemsRepository.DataTable.Rows)
                if ((string)x[_nameColumnName] == name)
                {
                    var it = new Data.Models.Item();
                    it.MapToModel(x);
                    return it;
                }

            return null;
        }
        public Data.Models.Item GetItem(int id)
        {
            foreach (DataRow x in _itemsRepository.DataTable.Rows)
                if ((int)x[_idColumnName] == id)
                {
                    var it = new Data.Models.Item();
                    it.MapToModel(x);
                    return it;
                }

            return null;
        }

        public Data.Models.ItemPrice GetItemPrice(int id, DateTime? dateTime)
        {
            if (dateTime != null)
                foreach (var x in _database.GetAll<Data.Models.ItemPrice>(null, null, String.Format("ItemRef = {0} AND Date <= '{1}'", id, dateTime.Value.ToString("yyyy-MM-dd")), "Date DESC", 1))
                    return x;

            var item = GetItem(id);
            return new Data.Models.ItemPrice { ItemRef = item.Id, Item = item, Date = dateTime == null ? DateTime.Now : dateTime.Value, Price = item.Price };
        }

        public void SaveItem(Data.Models.Item item)
        {
            _database.Save(item);
        }

    }
}
