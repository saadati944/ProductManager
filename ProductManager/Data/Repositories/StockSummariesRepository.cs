using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Repositories
{
    public class StockSummariesRepository : IRepository
    {
        //TODO: add a different table for total quantity
        private DataTable _dataTable = null;
        private readonly Database _database;
        private SqlConnection _connection;
        private SqlDataAdapter _dataAdapter;
        private readonly Business.ItemsBusiness _itemsBusiness;
        private readonly SellInvoicesRepository _sellInvoicesRepository;
        private readonly BuyInvoicesRepository _buyInvoicesRepository;
        private readonly MeasurementUnitsRepository _measurementUnitsRepository;

        //private List<ItemQuantity> _itemTotalQuantities = new List<ItemQuantity>();

        private const string _stockRefColumnName = "StockRef";
        private const string _stockColumnName = "Stock";
        private const string _itemRefColumnName = "ItemRef";
        private const string _quantityColumnName = "Quantity";
        private const string _itemColumnName = "Item";
        private const string _measurementUnitColumnName = "MeasurementUnit";

        public DataTable DataTable
        {
            get
            {
                if (_dataTable == null)
                    Update();
                return _dataTable;
            }
        }

        public event DataChangeHandler DataChanged;

        public StockSummariesRepository(Database database, Business.ItemsBusiness itemsBusiness, SellInvoicesRepository sellInvoicesRepository, BuyInvoicesRepository buyInvoicesRepository, MeasurementUnitsRepository measurementUnitsRepository)
        {
            _database = database;
            _itemsBusiness = itemsBusiness;
            _sellInvoicesRepository = sellInvoicesRepository;
            _buyInvoicesRepository = buyInvoicesRepository;
            _measurementUnitsRepository = measurementUnitsRepository;

            _itemsBusiness.ItemsRepository.DataChanged += Update;
            _sellInvoicesRepository.DataChanged += Update;
            _buyInvoicesRepository.DataChanged += Update;
        }

        public void Update()
        {
            //_itemTotalQuantities.Clear();
            if (_connection == null)
            {
                _connection = _database.GetConnection();
                _dataAdapter = _database.GetDataAdapter<Models.StockSummary>(_connection);
            }
            if (_dataTable == null)
                _dataTable = new DataTable();
            else
                _dataTable.Clear();

            _dataAdapter.Fill(_dataTable);

            if (!_dataTable.Columns.Contains(_stockColumnName))
                _dataTable.Columns.Add(_stockColumnName);
            if (!_dataTable.Columns.Contains(_itemColumnName))
                _dataTable.Columns.Add(_itemColumnName);
            if (!_dataTable.Columns.Contains(_measurementUnitColumnName))
                _dataTable.Columns.Add(_measurementUnitColumnName);

            for (int i = 0; i < _dataTable.Rows.Count; i++)
            {
                DataRow dr = _dataTable.Rows[i];

                Models.Item item = _itemsBusiness.GetItemModel((int)dr[_itemRefColumnName]);
                dr[_itemColumnName] = item;
                dr[_measurementUnitColumnName] = _measurementUnitsRepository.GetUnit(item.MeasurementUnitRef);

                Models.Stock stock = new Models.Stock { Id = (int)dr[_stockRefColumnName] };
                stock.Load();
                dr[_stockColumnName] = stock;
                //AddItemQuantity((int)dr[_itemRefColumnName], (int)dr[_quantityColumnName], (string)dr[_measurementUnitColumnName]);
            }

            //foreach(var x in _itemTotalQuantities)
            //{
            //    Models.Item item = _itemsBusiness.GetItemModel(x.ItemRef);
            //    _dataTable.Rows.Add(-1, x.ItemRef, -1, x.Quantity, "مجموع موجودی", item, x.MeasurementUnit);
            //}

            if (DataChanged != null)
                DataChanged();
        }

        //private void AddItemQuantity(int itemref, int quantity, string unit)
        //{
        //    foreach(var x in _itemTotalQuantities)
        //        if(x.ItemRef == itemref)
        //        {
        //            x.Quantity += quantity;
        //            return;
        //        }

        //    _itemTotalQuantities.Add(new ItemQuantity { ItemRef = itemref, Quantity = quantity, MeasurementUnit = unit });
        //}

        private class ItemQuantity
        {
            public int Quantity { get; set; }
            public int ItemRef { get; set; }
            public string MeasurementUnit { get; set; }
        }
    }
}
