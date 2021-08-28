using System;
using BasicData.DataLayer.Models;
using Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicData.Interfaces
{
    public interface IItemPricesRepository : IRepository
    {
        IEnumerable<ItemPrice> ItemPrices { get; }
    }
}
