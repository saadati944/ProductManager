using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Commands
{
    public class UpdateCommand
    {
        private SqlCommand _sqlCommand;
        public UpdateCommand(SqlConnection connection, SqlTransaction transaction, string table, string[] columns, string[] values, string condition)
        {
            string query = CreateQuery(table, columns, values, condition);
            System.Windows.Forms.MessageBox.Show(query);
            if(transaction == null)
                _sqlCommand = new SqlCommand(query, connection);
            else
                _sqlCommand = new SqlCommand(query, connection, transaction);
            AddParameters(_sqlCommand, values);
        }
        public void Execute()
        {
            _sqlCommand.ExecuteNonQuery();
        }
        private string CreateQuery(string table, string[] columns, string[] values, string condition)
        {
            StringBuilder query = new StringBuilder("UPDATE ");
            query.Append(table);
            query.Append(" SET ");
            for (int i = 0; i < columns.Length; i++)
            {
                if (i != 0)
                    query.Append(", ");
                query.Append(String.Format("{0} = @PaRaM"+i, columns[i]));
            }
            query.Append(String.Format(" WHERE {0}; ", condition));
            return query.ToString();
        }
        private void AddParameters(SqlCommand command, string[] values)
        {
            for (int i = 0; i < values.Length; i++)
                command.Parameters.AddWithValue("@PaRaM" + i, values[i]);
        }
    }
}
