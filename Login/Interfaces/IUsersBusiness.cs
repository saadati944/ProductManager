using System;
using Framework.DataLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Interfaces
{
    public interface IUsersBusiness
    {
        IEnumerable<User> Users { get; }
        User Login(string username, string password);
    }
}
