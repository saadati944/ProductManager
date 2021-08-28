using BasicData.DataLayer.Models;
using Framework.DataLayer.Models;
using Framework.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BasicData.Repositories
{
    public class ItemsRepository : Interfaces.IItemsRepository
    {
        private DataTable _dataTable = null;
        private readonly Interfaces.IMeasurementUnitsRepository _measurementUnitsRepository;
        private readonly IDatabase _database;
        private SqlConnection _connection;
        private SqlDataAdapter _dataAdapter;


        public IEnumerable<Item> Items
        {
            get
            {
                if (_dataTable == null)
                    Update();
                return _dataTable.Select().Select(x => { var i = new Item(); i.MapToModel(x); return i; });
            }
        }
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

        public ItemsRepository(IDatabase database, Interfaces.IMeasurementUnitsRepository measurementUnitsRepository)
        {
            _database = database;
            _measurementUnitsRepository = measurementUnitsRepository;

            if (_measurementUnitsRepository.DataTable == null)
                _measurementUnitsRepository.Update();

            _measurementUnitsRepository.DataChanged += Update;
        }


        public void Update()
        {
            if (_connection == null)
            {
                _connection = _database.GetConnection();
                _dataAdapter = _database.GetDataAdapter<Item>(_connection);
            }
            if (_dataTable == null)
                _dataTable = new DataTable();
            else
                _dataTable.Clear();

            _dataAdapter.Fill(_dataTable);

            if (!_dataTable.Columns.Contains(Item.MeasurementUnitColumnName))
                _dataTable.Columns.Add(Item.MeasurementUnitColumnName, typeof(MeasurementUnit));
            if (!_dataTable.Columns.Contains(Item.CreatorColumnName))
                _dataTable.Columns.Add(Item.CreatorColumnName, typeof(User));

            for (int i = 0; i < _dataTable.Rows.Count; i++)
            {
                DataRow dr = _dataTable.Rows[i];
                dr[Item.MeasurementUnitColumnName] = _measurementUnitsRepository.GetUnit((int)dr[Item.MeasurementUnitRefColumnName]);
                User user = new User { Id = (int)dr[Item.CreatorRefColumnName] };
                _database.Load(user);
                dr[Item.CreatorColumnName] = user;
            }


            if (DataChanged != null)
                DataChanged();
        }

        public DataTable NewItemsDatatable()
        {
            return DataTable.Clone();
        }
        public Item GetItemModel(string name)
        {
            Item item = new Item();
            item.MapToModel(GetItem(name).Rows[0]);
            return item;
        }

        public Item GetItemModel(int id)
        {
            Item item = new Item();
            item.MapToModel(GetItem(id).Rows[0]);
            return item;
        }

        public DataTable GetItem(string name)
        {
            DataTable table = DataTable.Clone();

            foreach (DataRow x in DataTable.Rows)
                if (x.Field<string>(Item.NameColumnName) == name)
                {
                    table.Rows.Add(x.ItemArray);
                    break;
                }

            return table;
        }

        public DataTable GetItem(int id)
        {
            DataTable table = DataTable.Clone();

            foreach (DataRow x in DataTable.Rows)
                if (x.Field<int>(Item.IdColumnName) == id)
                {
                    table.Rows.Add(x.ItemArray);
                    break;
                }

            return table;
        }

    }
}
