using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace Business
{
    public class IOCRegistery : Registry
    {
        public IOCRegistery()
        {
            For<Repositories.BuyInvoicesRepository>().Singleton().Use<Repositories.BuyInvoicesRepository>();
            For<Repositories.SellInvoicesRepository>().Singleton().Use<Repositories.SellInvoicesRepository>();
            For<Repositories.MeasurementUnitsRepository>().Singleton().Use<Repositories.MeasurementUnitsRepository>();
            For<Repositories.ItemsRepository>().Singleton().Use<Repositories.ItemsRepository>();
        }
    }
}
