using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace DataLayer
{
    public class IOCRegistery : Registry
    {
        public IOCRegistery()
        {
            For<Database>().Singleton().Use<Database>();
        }
    }
}
