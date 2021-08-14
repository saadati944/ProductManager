using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Commands
{
    public class InsertCommand
    {
        private readonly SqlCommand _sqlCommand;
        public InsertCommand(SqlConnection connection, SqlTransaction transaction, string table, string[] columns, string[] values)
        {
            string query = CreateQuery(table, columns, values);
            System.Windows.Forms.MessageBox.Show(query);
            if(transaction != null)
                _sqlCommand = new SqlCommand(query, connection, transaction);
            else
                _sqlCommand = new SqlCommand(query, connection);
            AddParameters(_sqlCommand, values);
        }
        
        public int Execute()
        {
            return Convert.ToInt32(_sqlCommand.ExecuteScalar());
        }

        private string CreateQuery(string table, string[] columns, string[] values)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("INSERT INTO {0} ({1}) VALUES (", table, String.Join(", ", columns)));
            for (int i = 0; i < values.Length; i++)
            {
                if (i != 0)
                    sb.Append(", ");
                sb.Append("@PaRaM" + i);
            }
            
            sb.Append("); ");
            sb.Append("SELECT SCOPE_IDENTITY(); ");
            //sb.Append("DECLARE @id AS int = SCOPE_IDENTITY(); ");
            //sb.Append("SELECT @id; ");

            return sb.ToString();
        }
        private void AddParameters(SqlCommand command, string[] values)
        {
            for (int i = 0; i < values.Length; i++)
                command.Parameters.AddWithValue("@PaRaM" + i, values[i]);
        }
    }
}
