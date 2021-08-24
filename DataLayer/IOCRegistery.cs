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
