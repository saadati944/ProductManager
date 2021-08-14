using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Tappe.Data.Repositories
{
    public class ItemsRepository : IRepository
    {
        private DataTable _dataTable = null;
        private readonly MeasurementUnitsRepository _measurementUnitsRepository;
        private readonly Database _database;
        private SqlConnection _connection;
        private SqlDataAdapter _dataAdapter;

        private const string _itemMeasurementUnitColumnName = "MeasurementUnit";
        private const string _nameColumnName = "Name";
        private const string _idColumnName = "Id";
        private const string _itemMeasurementUnitRefColumnName = "MeasurementUnitRef";
        private const string _creatorColumnName = "Creator";
        private const string _creatorRefColumnName = "CreatorRef";

        public IEnumerable<Models.Item> Items
        {
            get
            {
                if (_dataTable == null)
                    Update();
                return _dataTable.Select().Select(x => { var i = new Models.Item(); i.MapToModel(x); return i; });
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
        
        public ItemsRepository()
        {
            _database = container.Create<Database>();
            _measurementUnitsRepository = container.Create<MeasurementUnitsRepository>();

            if (_measurementUnitsRepository.DataTable == null)
                _measurementUnitsRepository.Update();

            _measurementUnitsRepository.DataChanged += Update;
        }


        public void Update()
        {
            if (_connection == null)
            {
                _connection = _database.GetConnection();
                _dataAdapter = _database.GetDataAdapter<Models.Item>(_connection);
            }
            if(_dataTable == null)
                _dataTable = new DataTable();
            else
                _dataTable.Clear();

            _dataAdapter.Fill(_dataTable);

            if (!_dataTable.Columns.Contains(_itemMeasurementUnitColumnName))
                _dataTable.Columns.Add(_itemMeasurementUnitColumnName);
            if (!_dataTable.Columns.Contains(_creatorColumnName))
                _dataTable.Columns.Add(_creatorColumnName);

            for (int i=0; i<_dataTable.Rows.Count; i++)
            {
                DataRow dr = _dataTable.Rows[i];
                dr[_itemMeasurementUnitColumnName] = _measurementUnitsRepository.GetUnit((int)dr[_itemMeasurementUnitRefColumnName]);
                Models.User user = new Models.User { Id = (int)dr[_creatorRefColumnName] };
                user.Load();
                dr[_creatorColumnName] = user;
            }


            if (DataChanged != null)
                DataChanged();
        }
    }
}
