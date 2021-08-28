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
    public interface IItemsRepository : IRepository
    {
        IEnumerable<Item> Items { get; }
        DataTable NewItemsDatatable();
        Item GetItemModel(string name);
        Item GetItemModel(int id);
        DataTable GetItem(string name);
        DataTable GetItem(int id);

    }
}
