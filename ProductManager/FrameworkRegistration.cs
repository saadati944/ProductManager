using Framework.Interfaces;
using Framework.DataLayer;
using Framework.Business;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class FrameworkRegistration : IRegistration
    {
        public FrameworkRegistration()
        {
            InstanceScanner.RegisterSingleton<IDatabase, Database>();

            InstanceScanner.Register<IFormFactory, FormFactory>();
            InstanceScanner.Register<IPermissionsBusiness, PermissionsBusiness>();
            InstanceScanner.Register<ISettingsBusiness, SettingsBusiness>();
        }
    }
}
