using System;
using System.Data.SqlClient;

namespace DataLayer.Commands
{
    public class DeleteCommand
    {
        private SqlCommand _sqlCommand;

        public DeleteCommand(SqlConnection connection, SqlTransaction transaction, string table, string condition)
        {
            if (transaction == null)
                _sqlCommand = new SqlCommand(CreateQuery(table, condition), connection);
            else
                _sqlCommand = new SqlCommand(CreateQuery(table, condition), connection, transaction);
        }
        public DeleteCommand(SqlCommand sqlCommand, string table, string condition)
        {
            sqlCommand.CommandText += CreateQuery(table, condition);
        }
        public void execute()
        {
            _sqlCommand.ExecuteNonQuery();
        }

        private string CreateQuery(string table, string condition)
        {
            return String.Format("DELETE FROM {0} WHERE {1}; ", table, condition);
        }
    }
}
