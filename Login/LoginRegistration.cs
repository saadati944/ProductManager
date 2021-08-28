using Framework;
using Login.Interfaces;
using Login.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login
{
    class LoginRegistration : Framework.Interfaces.IRegistration
    {
        public LoginRegistration()
        {
            InstanceScanner.Register<IUsersBusiness, UsersBusiness>();
        }
    }
}
