using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Repositories
{
    class ItemPricesRepository : IRepository
    {
        private DataTable _dataTable;
        private readonly Database _database;
        private SqlConnection _connection;
        private SqlDataAdapter _dataAdapter;

        private readonly Business.ItemsBusiness _itemsBusiness;
        private readonly ItemsRepository _itemsRepository;

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
        public IEnumerable<Models.ItemPrice> ItemPrices
        {
            get
            {
                if (_dataTable == null)
                    Update();
                return _dataTable.Select().Select(x => { var i = new Models.ItemPrice(); i.MapToModel(x); return i; });
            }
        }

        public event DataChangeHandler DataChanged;

        public ItemPricesRepository()
        {
            _database = container.Create<Database>();
            _itemsBusiness = container.Create<Business.ItemsBusiness>();
            _itemsRepository = _itemsBusiness.ItemsRepository;


            _itemsRepository.DataChanged += Update;
        }

        public void Update()
        {
            if (_connection == null)
            {
                _connection = _database.GetConnection();
                _dataAdapter = _database.GetDataAdapter<Models.ItemPrice>(_connection);
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

                Models.Item item = _itemsBusiness.GetItemModel((int)dr[_itemRefColumnName]);
                dr[_itemColumnName] = item;

                dr[_persianDateColumnName] = Models.PersianDate.PersianDateStringFromDateTime((DateTime)dr[_dateColumnName]);
            }

            if (DataChanged != null)
                DataChanged();
        }
    }
}
