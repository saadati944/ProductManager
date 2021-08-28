using BasicData.DataLayer.Models;
using Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BasicData.Repositories
{
    public class ItemPricesRepository : Interfaces.IItemPricesRepository
    {
        private readonly IDatabase _database;
        private DataTable _dataTable;
        private SqlConnection _connection;
        private SqlDataAdapter _dataAdapter;

        private readonly Interfaces.IItemsRepository _itemsRepository;

        private const string _itemRefColumnName = "ItemRef";
        private const string _itemColumnName = "Item";
        private const string _dateColumnName = "Date";
        private const string _persianDateColumnName = "PersianDate";
        public DataTable DataTable
        {
            get
            {
                if (_dataTable == null)
                    Update();
                return _dataTable;
            }
        }
        public IEnumerable<ItemPrice> ItemPrices
        {
            get
            {
                if (_dataTable == null)
                    Update();
                return _dataTable.Select().Select(x => { var i = new ItemPrice(); i.MapToModel(x); return i; });
            }
        }

        public event DataChangeHandler DataChanged;

        public ItemPricesRepository(IDatabase database, Interfaces.IItemsRepository itemsRepository)
        {
            _database = database;
            _itemsRepository = itemsRepository;

            _itemsRepository.DataChanged += Update;
        }

        public void Update()
        {
            if (_connection == null)
            {
                _connection = _database.GetConnection();
                _dataAdapter = _database.GetDataAdapter<ItemPrice>(_connection);
            }
            if (_dataTable == null)
                _dataTable = new DataTable();
            else
                _dataTable.Clear();

            _dataAdapter.Fill(_dataTable);

            if (!_dataTable.Columns.Contains(_itemColumnName))
                _dataTable.Columns.Add(_itemColumnName);
            if (!_dataTable.Columns.Contains(_persianDateColumnName))
                _dataTable.Columns.Add(_persianDateColumnName);

            for (int i = 0; i < _dataTable.Rows.Count; i++)
            {
                DataRow dr = _dataTable.Rows[i];

                Item item = _itemsRepository.GetItemModel((int)dr[_itemRefColumnName]);
                dr[_itemColumnName] = item;

                dr[_persianDateColumnName] = Framework.Utilities.PersianDate.PersianDateStringFromDateTime((DateTime)dr[_dateColumnName]);
            }

            if (DataChanged != null)
                DataChanged();
        }
    }
}
