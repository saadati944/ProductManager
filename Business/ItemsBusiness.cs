using DataLayer;
using DataLayer.Models;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utilities;

namespace Business
{
    public class ItemsBusiness
    {
        private readonly Database _database;
        private readonly ItemsRepository _itemsRepository;


        public ItemsRepository ItemsRepository
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

        public ItemsBusiness(Database database, ItemsRepository itemsrepo)
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
                return IOC.Container.GetInstance<Database>().GetAll<StockSummary>(null, null, "StockRef=" + stockRef + " AND ItemRef=" + itemRef, null, 1).First().Quantity;
            }
            catch { }
            return 0;
        }

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
        //public Item GetItemModel(int id)
        //{
        //    //TODO: remove this function
        //    return _itemsRepository.GetItemModel(id);
        //}

        //public DataTable GetItem(int id)
        //{
        //    //TODO: remove this function
        //    return _itemsRepository.GetItem(id);
        //}


        public ItemPrice GetItemPrice(int id, DateTime? dateTime)
        {
            if (dateTime != null)
                foreach (var x in _database.GetAll<ItemPrice>(null, null, String.Format("ItemRef = {0} AND Date <= '{1}'", id, dateTime.Value.ToString("yyyy-MM-dd")), "Date DESC", 1))
                    return x;

            var item = _itemsRepository.GetItemModel(id);
            return new ItemPrice { ItemRef = item.Id, Item = item, Date = dateTime == null ? DateTime.Now : dateTime.Value, Price = item.Price };
        }

        //public DataTable NewTable()
        //{
        //    //TODO: remove this function
        //    return _itemsRepository.NewItemsDatatable();
        //}

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
