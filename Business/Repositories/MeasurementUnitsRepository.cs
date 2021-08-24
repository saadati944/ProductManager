using DataLayer;
using DataLayer.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Business.Repositories
{
    public class MeasurementUnitsRepository : IRepository
    {
        private DataTable _dataTable = null;
        private readonly Database _database;
        private SqlConnection _connection;
        private SqlDataAdapter _dataAdapter;

        public IEnumerable<MeasurementUnit> MeasurementUnits
        {
            get
            {
                if (_dataTable == null)
                    Update();
                return _dataTable.Select().Select(x => { var i = new MeasurementUnit(); i.MapToModel(x); return i; });
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

        public MeasurementUnitsRepository(Database database)
        {
            _database = database;
        }

        public MeasurementUnit GetUnit(string name)
        {
            for (int i = 0; i < DataTable.Rows.Count; i++)
                if (DataTable.Rows[i].Field<string>("Name") == name)
                {
                    var x = new MeasurementUnit();
                    x.MapToModel(DataTable.Rows[i]);
                    return x;
                }

            return null;
        }
        public MeasurementUnit GetUnit(int id)
        {
            for (int i = 0; i < DataTable.Rows.Count; i++)
                if (DataTable.Rows[i].Field<int>("Id") == id)
                {
                    var x = new MeasurementUnit();
                    x.MapToModel(DataTable.Rows[i]);
                    return x;
                }

            return null;
        }
        public void Update()
        {
            if (_connection == null)
            {
                _connection = _database.GetConnection();
                _dataAdapter = _database.GetDataAdapter<MeasurementUnit>(_connection);
            }
            if (_dataTable == null)
                _dataTable = new DataTable();
            else
                _dataTable.Clear();

            _dataAdapter.Fill(_dataTable);

            if (DataChanged != null)
                DataChanged();
        }
    }
}
