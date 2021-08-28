using System;
using System.Collections.Generic;
using StructureMap;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;

namespace Framework
{
    public static class InstanceScanner
    {
        public static void ScanProjects()
        {
            Utilities.IOC.Container = new Container(x =>
            {
                x.Scan(_ =>
                {
                    _.TheCallingAssembly();
                    _.AssembliesFromApplicationBaseDirectory();
                    // _.LookForRegistries();
                    _.AddAllTypesOf<Interfaces.IRegistration>();
                    _.WithDefaultConventions();
                });
                
            });
            ScanCustomRegisteries();
        }

        public static void RegisterSingleton<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : TInterface
        {
            Utilities.IOC.Container.Configure(_ =>
            {
                _.For<TInterface>().DecorateAllWith(x => GetDecorator(x));
                _.For<TInterface>().Singleton().Use<TImplementation>();
            });
        }
        public static void Register<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : TInterface
        {
            Utilities.IOC.Container.Configure(_ =>
            {
                _.For<TInterface>().DecorateAllWith(x => GetDecorator(x));
                _.For<TInterface>().Use<TImplementation>();
            });
        }

        private static T GetDecorator<T>(T target)
            where T: class
        {
            Castle.DynamicProxy.ProxyGenerator pg = new Castle.DynamicProxy.ProxyGenerator();
            return pg.CreateInterfaceProxyWithTarget(target, new LoggingInterceptor());
        }

        private static void ScanCustomRegisteries()
        {
            var unused = Utilities.IOC.Container.GetAllInstances<Interfaces.IRegistration>();
        }
    }
}
