using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Framework.DataLayer.Commands
{
    public class SelectCommand
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;
        private readonly string _table;
        private readonly string[] _columns;
        private readonly string _condition;
        private readonly string _orderby;
        private readonly int _top;
        private readonly bool _autoAddIdColumn;
        private readonly bool _autoAddVersionColumn;
        private readonly string _customeCommand = null;
        private readonly string[] _parameterNames = null;
        private readonly object[] _parameterValues;
        private DataSet _dataset;

        public SelectCommand(SqlConnection connection, SqlTransaction transaction, string table, string[] columns, string condition = null, string orderby = null, int top = -1, bool autoAddVersionColumn = false, bool autoAddIdColumn = true)
        {
            _connection = connection;
            _transaction = transaction;
            _table = table;
            _columns = columns;
            _condition = condition;
            _orderby = orderby;
            _top = top;
            _autoAddIdColumn = autoAddIdColumn;
            _autoAddVersionColumn = autoAddVersionColumn;
        }

        public SelectCommand(SqlConnection connection, string customeCommand, string[] parameterNames = null, object[] parameterValues = null, SqlTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
            _customeCommand = customeCommand;
            _parameterNames = parameterNames;
            _parameterValues = parameterValues;
        }

        public EnumerableRowCollection<DataRow> Execute()
        {
            execute();

            if (_dataset.Tables[0].Rows.Count == 0)
                return null;


            return _dataset.Tables[0].AsEnumerable();
        }

        public DataSet ExecuteDataset()
        {
            execute();
            return _dataset;
        }

        public SqlDataAdapter GetDataAdapter()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = _connection;
            sqlCommand.CommandText = GenerateQuery();
            SetParameters(sqlCommand);
            adapter.SelectCommand = sqlCommand;
            return adapter;
        }

        private void execute()
        {
            SqlCommand sqlCommand;
            string query = GenerateQuery();
            if (_transaction == null)
                sqlCommand = new SqlCommand(query, _connection);
            else
                sqlCommand = new SqlCommand(query, _connection, _transaction);

            SetParameters(sqlCommand);

            SqlDataAdapter dataadapter = new SqlDataAdapter();
            dataadapter.SelectCommand = sqlCommand;
            DataSet dataset = new DataSet();
            dataadapter.Fill(dataset);
            _dataset = dataset;
        }

        private void SetParameters(SqlCommand command)
        {
            if (_parameterNames != null)
                for (int i = 0; i < _parameterNames.Length; i++)
                    command.Parameters.AddWithValue(_parameterNames[i], _parameterValues[i]);
        }

        private string GenerateQuery()
        {
            if (_customeCommand == null)
                return String.Format("SELECT {0} {1} {2} {3} FROM {4} {5} {6};", _top == -1 ? "" : "TOP " + _top.ToString()
                    , _autoAddIdColumn ? "Id, " : "", _autoAddVersionColumn ? "Version, " : ""
                    , String.Join(", ", _columns), _table, _condition == null ? "" : "WHERE " + _condition, _orderby == null ? "" : "ORDER BY " + _orderby);

            return _customeCommand;
        }

    }
}
