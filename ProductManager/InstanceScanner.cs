using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace Tappe
{
    class InstanceScanner : Registry
    {
        public InstanceScanner()
        {
            Scan(_ =>
            { 
                _.TheCallingAssembly() ;
                _.WithDefaultConventions();
            });
            For<Data.Database>().Singleton().Use<Data.Database>();
            For<Data.Repositories.BuyInvoicesRepository>().Singleton().Use<Data.Repositories.BuyInvoicesRepository>();
            For<Data.Repositories.SellInvoicesRepository>().Singleton().Use<Data.Repositories.SellInvoicesRepository>();
            For<Data.Repositories.MeasurementUnitsRepository>().Singleton().Use<Data.Repositories.MeasurementUnitsRepository>();
            For<Data.Repositories.ItemsRepository>().Singleton().Use<Data.Repositories.ItemsRepository>();
        }
    }
}
