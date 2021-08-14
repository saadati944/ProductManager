using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe
{
    public static class container
    {
        private static Data.Database _database;
        private static Data.Repositories.MeasurementUnitsRepository _measurementUnitsRepository;
        private static Data.Repositories.ItemsRepository _itemsRepository;
        private static Data.Repositories.SellInvoicesRepository _sellInvoicesRepository;
        private static Data.Repositories.BuyInvoicesRepository _buyInvoicesRepository;
        public static T Create<T>()
            where T : new()
        {
            Type t = typeof(T);
            if (t == typeof(Data.Database))
            {
                if (_database == null)
                    _database = new Data.Database();

                return (T)Convert.ChangeType(_database, t);
            }

            if (t == typeof(Data.Repositories.ItemsRepository))
            {
                if (_itemsRepository == null)
                    _itemsRepository = new Data.Repositories.ItemsRepository();

                return (T)Convert.ChangeType(_itemsRepository, t);
            }

            if (t == typeof(Data.Repositories.MeasurementUnitsRepository))
            {
                if (_measurementUnitsRepository == null)
                    _measurementUnitsRepository = new Data.Repositories.MeasurementUnitsRepository();

                return (T)Convert.ChangeType(_measurementUnitsRepository, t);
            }

            if (t == typeof(Data.Repositories.SellInvoicesRepository))
            {
                if (_sellInvoicesRepository == null)
                    _sellInvoicesRepository = new Data.Repositories.SellInvoicesRepository();

                return (T)Convert.ChangeType(_sellInvoicesRepository, t);
            }

            if (t == typeof(Data.Repositories.BuyInvoicesRepository))
            {
                if (_buyInvoicesRepository == null)
                    _buyInvoicesRepository = new Data.Repositories.BuyInvoicesRepository();

                return (T)Convert.ChangeType(_buyInvoicesRepository, t);
            }

            return new T();
        }

        public static void Reset()
        {
            _database = null;
            _measurementUnitsRepository = null;
            _itemsRepository = null;
            _sellInvoicesRepository = null;
            _buyInvoicesRepository = null;
        }
    }
}
