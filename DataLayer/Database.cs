using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DataLayer
{
    public class Database
    {
        private const string _ConnectionString = @"Server=VDI-MIS-077\SEPIDAR;Database=adodotnet;User Id=test;Password=!@qw3456; ";

        public static Models.User LoggedInUser;

        public enum SaveMode
        {
            Auto,
            StoreIdInVariable
        }

        private readonly SqlConnection _connection;
        //private SqlCommand _sqlCommand = null;
        //private bool _inTransaction = false;

        private bool isOpen = false;

        public Database()
        {
            _connection = new SqlConnection(_ConnectionString);
            Open();
        }
        private void Open()
        {
            if (!isOpen)
                _connection.Open();
            isOpen = true;
        }
        private void Close()
        {
            if (isOpen)
                _connection.Close();
            isOpen = false;
        }

        public System.Data.DataSet GetAllDataset<T>(SqlConnection connection = null, SqlTransaction transaction = null, string condition = null, string orderby = null, int top = -1)
            where T : Models.Model, new()
        {
            T temp = new T();
            return new Commands.SelectCommand(connection == null ? _connection : connection, transaction, temp.TableName(), temp.Columns(), condition, orderby, top, temp is Models.VersionableModel).ExecuteDataset();
        }

        public IEnumerable<T> GetAll<T>(SqlConnection connection = null, SqlTransaction transaction = null, string condition = null, string orderby = null, int top = -1)
            where T : Models.Model, new()
        {
            T temp = new T();
            EnumerableRowCollection<DataRow> rows = new Commands.SelectCommand(connection == null ? _connection : connection, transaction, temp.TableName(), temp.Columns(), condition, orderby, top, temp is Models.VersionableModel).Execute();
            if (rows == null)
                return Enumerable.Empty<T>();
            return rows.Select(x => { T t = new T(); t.MapToModel(x); return t; });
        }

        //TODO: use transaction in these functions
        public SqlDataAdapter GetDataAdapter<T>(SqlConnection connection, string condition = null, string orderby = null, int top = -1)
            where T : Models.Model, new()
        {
            T temp = new T();
            return new Commands.SelectCommand(connection, null, temp.TableName(), temp.Columns(), condition, orderby, top, temp is Models.VersionableModel).GetDataAdapter();
        }

        public DataSet CustomeQuery(string query, string[] parameterNames = null, object[] parameterValues = null, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return new Commands.SelectCommand(connection == null ? _connection : connection, query, parameterNames, parameterValues, transaction).ExecuteDataset();
        }


        public IEnumerable<Models.Item> Items
        {
            get
            {
                return GetAll<Models.Item>();
            }
        }

        public IEnumerable<Models.User> Users
        {
            get
            {
                return GetAll<Models.User>();
            }
        }

        public IEnumerable<Models.SellInvoice> SellInvoices
        {
            get
            {
                return GetAll<Models.SellInvoice>();
            }
        }

        public IEnumerable<Models.SellInvoiceItem> SellInvoiceItems
        {
            get
            {
                return GetAll<Models.SellInvoiceItem>();
            }
        }

        public IEnumerable<Models.BuyInvoice> BuyInvoices
        {
            get
            {
                return GetAll<Models.BuyInvoice>();
            }
        }

        public IEnumerable<Models.BuyInvoiceItem> BuyInvoiceItems
        {
            get
            {
                return GetAll<Models.BuyInvoiceItem>();
            }
        }

        public IEnumerable<Models.MeasurementUnit> MeasurementUnits
        {
            get
            {
                return GetAll<Models.MeasurementUnit>();
            }
        }

        public IEnumerable<Models.Party> Parties
        {
            get
            {
                return GetAll<Models.Party>();
            }
        }

        public IEnumerable<Models.Stock> Stocks
        {
            get
            {
                return GetAll<Models.Stock>();
            }
        }

        public void Save(Models.Model val, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            if (val.Id == -1)
                val.Id = new Commands.InsertCommand(connection == null ? _connection : connection, transaction, val.TableName(), val.Columns(), val.GetValues()).Execute();
            else
                new Commands.UpdateCommand(connection == null ? _connection : connection, transaction, val.TableName(), val.Columns(), val.GetValues(), "Id = " + val.Id).Execute();
        }
        /*
                 public void Save(Models.Model val, bool storeInIdVar = false, int idVarIndex = -1)
        {
            string[] values = val.GetValues();

            bool escape = true;
            if (idVarIndex != -1)
            {
                escape = false;
                for (int i = 0; i < values.Length; i++)
                {
                    if (i == idVarIndex)
                        values[0] = "@id";
                    else
                        values[i] = $"'{values[i].Replace("'", "''")}'";
                }
            }

            if (_inTransaction)
            {
                if (val.Id == -1)
                {
                    new Commands.InsertCommand(_sqlCommand, val.TableName(), val.Columns(), values, storeInIdVar, escape);
                }
                else
                    new Commands.UpdateCommand(_sqlCommand, val.TableName(), val.Columns(), values, $"Id = {val.Id}", escape);
            }
            else
            {
                if (val.Id == -1)
                    val.Id = new Commands.InsertCommand(_connection, val.TableName(), val.Columns(), values, escape).Execute();
                else
                    new Commands.UpdateCommand(_connection, val.TableName(), val.Columns(), values, $"Id = {val.Id}", escape).execute();
            }
        }
        */

        public void Load(Models.Model model, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            var select = new Commands.SelectCommand(connection == null ? _connection : connection, transaction, model.TableName(), model.Columns(), "id = " + model.Id, null, 1, model is Models.VersionableModel, false);
            DataRow row = select.Execute().First();
            model.MapToModel(row);
            return;
        }

        public void Delete(Models.Model model, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            //if(_inTransaction)
            //    new Commands.DeleteCommand(_sqlCommand, model.TableName(), $"Id = {model.Id}");
            //else
            //    new Commands.DeleteCommand(_connection, model.TableName(), $"Id = {model.Id}").execute();
            new Commands.DeleteCommand(connection == null ? _connection : connection, transaction, model.TableName(), "Id = " + model.Id).execute();
        }

        public SqlTransaction BeginTransaction(SqlConnection connection)
        {
            return connection.BeginTransaction();
        }
        public void CommitTransaction(SqlTransaction transaction)
        {
            transaction.Commit();
        }
        public void RollbackTransaction(SqlTransaction transaction)
        {
            transaction.Rollback();
        }

        /*
         public void BeginTransaction()
        {
            _inTransaction = true;
            _sqlCommand = new SqlCommand();
            _sqlCommand.Connection = _connection;
            _sqlCommand.CommandText = @"BEGIN TRY BEGIN TRANSACTION; ";
        }
        public bool CommitTransaction()
        {
            _sqlCommand.CommandText += @"COMMIT TRANSACTION; SELECT 1; END TRY BEGIN CATCH ROLLBACK TRANSACTION; SELECT 0; END CATCH";
            _inTransaction = false;
            //log(_sqlCommand.CommandText);
            bool result = (int)_sqlCommand.ExecuteScalar() == 1;
            _sqlCommand = null;
            return result;
        }
             */


        //private void log(string mes)
        //{
        //    int fn = 0;
        //    while (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "QUERY_" + fn+ ".sql")))
        //        fn++;
        //    System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "QUERY_" + fn + ".sql"), mes);
        //}

        public SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(_ConnectionString);
            connection.Open();
            return connection;
        }
    }
    public static class DatabaseExtensions
    {
        public static void Save(this Models.Model model)
        {
            Utilities.IOC.Container.GetInstance<Database>().Save(model);
        }

        public static void Load(this Models.Model model)
        {
            Utilities.IOC.Container.GetInstance<Database>().Load(model);
        }

        public static void Delete(this Models.Model model)
        {
            Utilities.IOC.Container.GetInstance<Database>().Delete(model);
        }
    }
}