using System;
using System.Data;
using Framework.DataLayer;
using Framework.Interfaces;
using Framework.DataLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Business
{
    public class UsersBusiness : Interfaces.IUsersBusiness
    {
        private readonly IDatabase _database;

        public IEnumerable<User> Users
        {
            get
            {
                return _database.GetAll<User>();
            }
        }

        public UsersBusiness(IDatabase database)
        {
            _database = database;
        }

        public DataTable NewDataTable()
        {
            return _database.GetAllDataset<User>(null, null, null, null, 0).Tables[0];
        }
        public bool ValidateDataTable(DataTable table)
        {
            bool res = true;
            for(int i=0; i<table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                row.ClearErrors();
                if (row[User.FirstnameColumnName] is DBNull || String.IsNullOrEmpty((string)row[User.FirstnameColumnName]))
                {
                    row.SetColumnError(User.FirstnameColumnName, "نام اجباری میباشد");
                    res = false;
                }
                if (row[User.LastnameColumnName] is DBNull || String.IsNullOrEmpty((string)row[User.LastnameColumnName]))
                {
                    row.SetColumnError(User.LastnameColumnName, "نام خانوادگی اجباری میباشد");
                    res = false;
                }
                if (row[User.AgenameColumnName] is DBNull)
                {
                    row.SetColumnError(User.AgenameColumnName, "سن کاربر اجباری میباشد");
                    res = false;
                }
                if (row[User.GendernameColumnName] is DBNull)
                {
                    row.SetColumnError(User.GendernameColumnName, "جنسیت کاربر اجباری میباشد");
                    res = false;
                }
                if (row[User.UsernameColumnName] is DBNull || String.IsNullOrEmpty((string)row[User.UsernameColumnName]))
                {
                    row.SetColumnError(User.LastnameColumnName, "نام کاربری اجباری میباشد");
                    res = false;
                }
                else if(_database.GetAllDataset<User>(null, null, String.Format("{0}='{1}'", User.UsernameColumnName, (string)row[User.UsernameColumnName]), null, 1).Tables[0].Rows.Count != 0)
                {
                    row.SetColumnError(User.LastnameColumnName, "نام کاربری تکراری میباشد");
                    res = false;
                }

            }
            return res;
        }

        public User Login(string username, string password)
        {
            return Framework.Utilities.LoggedInUser.User = _database.GetAll<User>(null, null, 
                String.Format("{0}='{1}' AND {2}='{3}'", User.UsernameColumnName, username, User.PasswordnameColumnName, password), null, 1).FirstOrDefault();
        }
    }
}
