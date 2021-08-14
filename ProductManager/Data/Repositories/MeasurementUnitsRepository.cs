using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Tappe.Data.Repositories
{
    public class MeasurementUnitsRepository : IRepository
    {
        private DataTable _dataTable = null;
        private readonly Database _database;
        private SqlConnection _connection;
        private SqlDataAdapter _dataAdapter;

        public IEnumerable<Models.MeasurementUnit> MeasurementUnits
        {
            get
            {
                if (_dataTable == null)
                    Update();
                return _dataTable.Select().Select(x => { var i = new Models.MeasurementUnit(); i.MapToModel(x); return i; });
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

        public MeasurementUnitsRepository()
        {
            _database = container.Create<Database>();
        }

        public Models.MeasurementUnit GetUnit(string name)
        {
            for(int i=0; i<DataTable.Rows.Count; i++)
                if(DataTable.Rows[i].Field<string>("Name") == name)
                {
                    var x = new Models.MeasurementUnit();
                    x.MapToModel(DataTable.Rows[i]);
                    return x;
                }

            return null;
        }
        public Models.MeasurementUnit GetUnit(int id)
        {
            for (int i = 0; i < DataTable.Rows.Count; i++)
                if (DataTable.Rows[i].Field<int>("Id") == id)
                {
                    var x = new Models.MeasurementUnit();
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
                _dataAdapter = _database.GetDataAdapter<Models.MeasurementUnit>(_connection);
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
