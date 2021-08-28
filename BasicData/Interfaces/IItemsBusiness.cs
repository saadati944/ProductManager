using System;
using System.Data;
using BasicData.DataLayer.Models;
using Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicData.Interfaces
{
    public interface IItemsBusiness
    {
        //TODO : remove this
        IEnumerable<Item> Items { get; }
        bool IsItemNameExists(string name);
        int GetItemQuantity(int itemRef, int stockRef);
        string[] GetItemNamesInStock(int stockref);
        Item GetItemModel(string name);
        ItemPrice GetItemPrice(int id, DateTime? dateTime);
        DatabaseSaveResult SaveItem(Item item);
        DatabaseSaveResult SaveItem(DataTable table);
        bool ValidateDataTable(DataTable table);
    }
}
