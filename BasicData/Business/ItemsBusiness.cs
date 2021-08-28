using BasicData.DataLayer.Models;
using Framework.DataLayer.Models;
using Framework.Interfaces;
using BasicData.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System;

namespace BasicData.Business
{
    public class ItemsBusiness : IItemsBusiness
    {
        private readonly IDatabase _database;
        private readonly IItemsRepository _itemsRepository;

        public IEnumerable<Item> Items
        {
            get
            {
                return _database.GetAll<Item>();
            }
        }

        public ItemsBusiness(IDatabase database, IItemsRepository itemsrepo)
        {
            _database = database;
            _itemsRepository = itemsrepo;
        }
        public bool IsItemNameExists(string name)
        {
            return _database.GetAll<Item>(null, null, "Name='" + name.Replace("'", "''") + "'", null, 1).Count() != 0;
        }

        public int GetItemQuantity(int itemRef, int stockRef)
        {
            var stockSummary=_database.GetAll<StockSummary>(null, null, "StockRef=" + stockRef + " AND ItemRef=" + itemRef, null, 1).FirstOrDefault();
            return stockSummary == null ? 0 : stockSummary.Quantity;
        }

        //TODO: rename to getStockItems then move to a business in the buyandsell module
        public string[] GetItemNamesInStock(int stockref)
        {
            var stockSummaries = _database.GetAll<StockSummary>(null, null, "StockRef=" + stockref);
            List<string> items = new List<string>();
            foreach (var x in stockSummaries)
                if (x.Quantity > 0)
                    items.Add((string)_itemsRepository.GetItem(x.ItemRef).Rows[0][Item.NameColumnName]);
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

        public ItemPrice GetItemPrice(int id, DateTime? dateTime)
        {
            if (dateTime != null)
                foreach (var x in _database.GetAll<ItemPrice>(null, null, String.Format("ItemRef = {0} AND Date <= '{1}'", id, dateTime.Value.ToString("yyyy-MM-dd")), "Date DESC", 1))
                    return x;

            var item = _itemsRepository.GetItemModel(id);
            return new ItemPrice { ItemRef = item.Id, Item = item, Date = dateTime == null ? DateTime.Now : dateTime.Value, Price = item.Price };
        }

        public DatabaseSaveResult SaveItem(Item item)
        {
            return _database.Save(item);
        }
        public DatabaseSaveResult SaveItem(DataTable table)
        {
            Item item = new Item();
            item.MapToModel(table.Rows[0]);
            return SaveItem(item);
        }

    }
}
