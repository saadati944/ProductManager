using Framework;
using BasicData.Interfaces;
using BasicData.Repositories;
using BasicData.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicData
{
    public class BasicDataRegistration : Framework.Interfaces.IRegistration
    {
        public BasicDataRegistration()
        {
            InstanceScanner.Register<IItemsBusiness, ItemsBusiness>();

            InstanceScanner.RegisterSingleton<IItemPricesRepository, ItemPricesRepository>();
            InstanceScanner.RegisterSingleton<IItemsRepository, ItemsRepository>();
            InstanceScanner.RegisterSingleton<IMeasurementUnitsRepository, MeasurementUnitsRepository>();
            InstanceScanner.RegisterSingleton<IStockSummariesRepository, StockSummariesRepository>();
        }
    }
}
