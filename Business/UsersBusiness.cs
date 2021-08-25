using System;
using DataLayer;
using DataLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class UsersBusiness
    {
        private readonly Database _database;
        public UsersBusiness(Database database)
        {
            _database = database;
        }

        public User Login(string username, string password)
        {
            return _database.GetAll<User>(null, null, String.Format("{0}='{1}' AND {2}='{3}'", User.UsernameColumnName, username, User.PasswordnameColumnName, password), null, 1).FirstOrDefault();
        }
    }
}
