using BasicData.DataLayer.Models;
using Framework.Interfaces;
using BasicData.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace BasicData.Repositories
{
    public class StockSummariesRepository : IStockSummariesRepository
    {
        //TODO: add a different table for total quantity
        private DataTable _dataTable = null;
        private readonly IDatabase _database;
        private SqlConnection _connection;
        private SqlDataAdapter _dataAdapter;
        private readonly IItemsRepository _itemsRepository;
        private readonly IMeasurementUnitsRepository _measurementUnitsRepository;

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

        public StockSummariesRepository(IDatabase database, IItemsRepository itemsRepository, IMeasurementUnitsRepository measurementUnitsRepository)
        {
            _database = database;
            _itemsRepository = itemsRepository;
            _measurementUnitsRepository = measurementUnitsRepository;

            _itemsRepository.DataChanged += Update;
        }

        public void Update()
        {
            //_itemTotalQuantities.Clear();
            if (_connection == null)
            {
                _connection = _database.GetConnection();
                _dataAdapter = _database.GetDataAdapter<StockSummary>(_connection);
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

                Item item = _itemsRepository.GetItemModel((int)dr[_itemRefColumnName]);
                dr[_itemColumnName] = item;
                dr[_measurementUnitColumnName] = _measurementUnitsRepository.GetUnit(item.MeasurementUnitRef);

                Stock stock = new Stock { Id = (int)dr[_stockRefColumnName] };
                _database.Load(stock);
                dr[_stockColumnName] = stock;
                //AddItemQuantity((int)dr[_itemRefColumnName], (int)dr[_quantityColumnName], (string)dr[_measurementUnitColumnName]);
            }

            //foreach(var x in _itemTotalQuantities)
            //{
            //    Item item = _itemsBusiness.GetItemModel(x.ItemRef);
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
