using Framework.DataLayer.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Framework.Interfaces
{
    public interface IDatabase
    {
        DataSet GetAllDataset<T>(SqlConnection connection = null, SqlTransaction transaction = null, string condition = null, string orderby = null, int top = -1)
            where T : Model, new();
        IEnumerable<T> GetAll<T>(SqlConnection connection = null, SqlTransaction transaction = null, string condition = null, string orderby = null, int top = -1)
            where T : Model, new();
        SqlDataAdapter GetDataAdapter<T>(SqlConnection connection, string condition = null, string orderby = null, int top = -1)
            where T : Model, new();
        DataSet CustomeQuery(string query, string[] parameterNames = null, object[] parameterValues = null, SqlConnection connection = null,
            SqlTransaction transaction = null);
        DatabaseSaveResult Save(Model val, SqlConnection connection = null, SqlTransaction transaction = null);
        void Load(Model model, SqlConnection connection = null, SqlTransaction transaction = null);
        void Delete(Model model, SqlConnection connection = null, SqlTransaction transaction = null);
        byte[] GetVersion<T>(int id, SqlConnection connection = null, SqlTransaction transaction = null);
        SqlTransaction BeginTransaction(SqlConnection connection);
        void CommitTransaction(SqlTransaction transaction);
        void RollbackTransaction(SqlTransaction transaction);
        SqlConnection GetConnection();
    }
}