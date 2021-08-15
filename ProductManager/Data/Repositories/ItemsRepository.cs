using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Tappe.Data.Models;

namespace Tappe.Data.Repositories
{
    public class ItemsRepository : IRepository
    {
        private DataTable _dataTable = null;
        private readonly MeasurementUnitsRepository _measurementUnitsRepository;
        private readonly Database _database;
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
                _dataAdapter = _database.GetDataAdapter<Item>(_connection);
            }
            if(_dataTable == null)
                _dataTable = new DataTable();
            else
                _dataTable.Clear();

            _dataAdapter.Fill(_dataTable);
            
            if (!_dataTable.Columns.Contains(Item.MeasurementUnitColumnName))
                _dataTable.Columns.Add(Item.MeasurementUnitColumnName, typeof(MeasurementUnit));
            if (!_dataTable.Columns.Contains(Item.CreatorColumnName))
                _dataTable.Columns.Add(Item.CreatorColumnName, typeof(User));

            for (int i=0; i<_dataTable.Rows.Count; i++)
            {
                DataRow dr = _dataTable.Rows[i];
                dr[Item.MeasurementUnitColumnName] = _measurementUnitsRepository.GetUnit((int)dr[Item.MeasurementUnitRefColumnName]);
                User user = new User { Id = (int)dr[Item.CreatorRefColumnName] };
                user.Load();
                dr[Item.CreatorColumnName] = user;
            }


            if (DataChanged != null)
                DataChanged();
        }
    }
}
