using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Models;
using System.Data;
using Utilities;

namespace Business
{
    public class ItemsBusiness
    {
        private readonly Database _database;
        private readonly Repositories.ItemsRepository _itemsRepository;


        public Repositories.ItemsRepository ItemsRepository
        {
            get
            {
                return _itemsRepository;
            }
        }

        public IEnumerable<Item> Items
        {
            get
            {
                return _database.Items;
            }
        }

        public ItemsBusiness(Database database, Repositories.ItemsRepository itemsrepo)
        {
            _database = database;
            _itemsRepository = itemsrepo;
        }
        public bool IsItemNameExists(string name)
        {
            return _database.GetAll<Item>(null, null, "Name='" + name.Replace("'", "''") + "'", null, 1).Count() != 0;
        }

        public static int GetItemQuantity(int itemRef, int stockRef)
        {
            try
            {
                return IOC.Container.GetInstance<Database>().GetAll<StockSummary>(null, null, "StockRef="+stockRef+" AND ItemRef=" + itemRef, null, 1).First().Quantity;
            }
            catch { }
            return 0;
        }

        public string[] GetItemNamesInStock(int stockref)
        {
            var stockSummaries = _database.GetAll<StockSummary>(null, null, "StockRef=" + stockref);
            List<string> items = new List<string>();
            foreach(var x in stockSummaries)
                if(x.Quantity > 0)
                    items.Add((string) GetItem(x.ItemRef).Rows[0][Item.NameColumnName]);
            return items.ToArray();
        }

        public Item GetItemModel(string name)
        {
            foreach (DataRow x in _itemsRepository.DataTable.Rows)
                if ((string)x[Item.NameColumnName] == name)
                {
                    var it = new Item();
                    it.MapToModel(x);
                    return it;
                }

            return null;
        }
        public Item GetItemModel(int id)
        {
            foreach (DataRow x in _itemsRepository.DataTable.Rows)
                if ((int)x[Item.IdColumnName] == id)
                {
                    var it = new Item();
                    it.MapToModel(x);
                    return it;
                }

            return null;
        }

        public DataTable GetItem(int id)
        {
            DataTable table = _itemsRepository.DataTable.Clone();
            
            foreach (DataRow x in _itemsRepository.DataTable.Rows)
                if ((int)x[Item.IdColumnName] == id)
                {
                    table.Rows.Add(x.ItemArray);
                    break;
                }

            return table;
        }


        public ItemPrice GetItemPrice(int id, DateTime? dateTime)
        {
            if (dateTime != null)
                foreach (var x in _database.GetAll<ItemPrice>(null, null, String.Format("ItemRef = {0} AND Date <= '{1}'", id, dateTime.Value.ToString("yyyy-MM-dd")), "Date DESC", 1))
                    return x;

            var item = GetItemModel(id);
            return new ItemPrice { ItemRef = item.Id, Item = item, Date = dateTime == null ? DateTime.Now : dateTime.Value, Price = item.Price };
        }

        public DataTable NewTable()
        {
            return _itemsRepository.DataTable.Clone();
        }

        public void SaveItem(Item item)
        {
            _database.Save(item);
        }
        public void SaveItem(DataTable table)
        {
            Item item = new Item();
            item.MapToModel(table.Rows[0]);
            _database.Save(item);
        }

    }
}
