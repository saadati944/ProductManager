using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace Framework
{
    static class InstanceScanner
    {
        public static void ScanProjects()
        {
            Utilities.IOC.Container = new Container(x =>
            {
                x.Scan(_ =>
                {
                    _.TheCallingAssembly();
                    _.WithDefaultConventions();
                });
                x.AddRegistry<DataLayer.IOCRegistery>();
                x.AddRegistry<Business.IOCRegistery>();
            });
        }
    }
}
