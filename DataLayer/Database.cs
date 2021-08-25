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

        public void Load(Models.Model model, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            var select = new Commands.SelectCommand(connection == null ? _connection : connection, transaction, model.TableName(), model.Columns(), "id = " + model.Id, null, 1, model is Models.VersionableModel, false);
            DataRow row = select.Execute().First();
            model.MapToModel(row);
            return;
        }

        public void Delete(Models.Model model, SqlConnection connection = null, SqlTransaction transaction = null)
        {
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