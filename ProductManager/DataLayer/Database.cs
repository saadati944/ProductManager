using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Framework.Interfaces;

namespace Framework.DataLayer
{
    public class Database : IDatabase
    {
        private const string _ConnectionString = @"Server=VDI-MIS-077\SEPIDAR;Database=adodotnet;User Id=test;Password=!@qw3456; ";

        private readonly SqlConnection _connection;

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

        public DataSet GetAllDataset<T>(SqlConnection connection = null, SqlTransaction transaction = null, string condition = null, string orderby = null, int top = -1)
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



        public DatabaseSaveResult Save(Models.Model val, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            if (val.Id == -1)
            {
                val.Id = new Commands.InsertCommand(connection == null ? _connection : connection, transaction, val.TableName(), val.Columns(), val.GetValues()).Execute();
                return DatabaseSaveResult.Saved;
            }
            else
            {
                if (val is Models.VersionableModel)
                {
                    byte[] versin = GetVersion(val.GetType(), val.Id, connection, transaction);
                    if (!Utilities.ArrayComparator.AreEqual(versin, ((Models.VersionableModel)val).Version))
                        return DatabaseSaveResult.AlreadyChanged;
                }

                new Commands.UpdateCommand(connection == null ? _connection : connection, transaction, val.TableName(), val.Columns(), val.GetValues(), "Id = " + val.Id).Execute();
                return DatabaseSaveResult.Updated;
            }
        }

        public byte[] GetVersion<T>(int id, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return GetVersion(typeof(T), id, connection, transaction);
        }
        private byte[] GetVersion(Type versionablemodel, int id, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            Models.VersionableModel t = (Models.VersionableModel) versionablemodel.GetConstructor(new Type[] { }).Invoke(null);
            string query = string.Format("SELECT Version FROM {0} WHERE Id={1}", t.TableName(), id);
            return (byte[])CustomeQuery(query, null, null, connection, transaction).Tables[0].Rows[0][0];
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
}