using StructureMap;

namespace DataLayer
{
    public class IOCRegistery : Registry
    {
        public IOCRegistery()
        {
            For<Database>().Singleton().Use<Database>();
            For<Repositories.BuyInvoicesRepository>().Singleton().Use<Repositories.BuyInvoicesRepository>();
            For<Repositories.SellInvoicesRepository>().Singleton().Use<Repositories.SellInvoicesRepository>();
            For<Repositories.MeasurementUnitsRepository>().Singleton().Use<Repositories.MeasurementUnitsRepository>();
            For<Repositories.ItemsRepository>().Singleton().Use<Repositories.ItemsRepository>();
            For<Repositories.StockSummariesRepository>().Singleton().Use<Repositories.StockSummariesRepository>();
        }
    }
}
