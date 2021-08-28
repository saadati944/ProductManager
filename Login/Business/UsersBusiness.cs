using System;
using Framework.DataLayer;
using Framework.Interfaces;
using Framework.DataLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Business
{
    public class UsersBusiness : Login.Interfaces.IUsersBusiness
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

        public User Login(string username, string password)
        {
            return Framework.Utilities.LoggedInUser.User = _database.GetAll<User>(null, null, 
                String.Format("{0}='{1}' AND {2}='{3}'", User.UsernameColumnName, username, User.PasswordnameColumnName, password), null, 1).FirstOrDefault();
        }
    }
}
