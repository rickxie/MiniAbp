using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using MiniAbp.DataAccess;
using IContainer = Autofac.IContainer;

namespace MiniAbp.Dependency
{
    public class IocManager 
    {
        public static IocManager Instance { get; private set; }
        private IContainer IocContainer { get; set; }
        private ContainerBuilder IocBuilder { get; set; }
        static IocManager()
        {
            Instance = new IocManager();
        }

        public IRegistrationBuilder<TImplementer, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<TImplementer>()
        {
            return IocBuilder.RegisterType<TImplementer>();
        }

        public T ResolveNamed<T>(string serviceName, params Parameter[] param)
        {
            return IocContainer.ResolveNamed<T>(serviceName, param);
        }

        public bool IsRegistered(Type type)
        {
            return IocContainer.IsRegistered(type);
        }
        public T Resolve<T>()
        {
            return IocContainer.Resolve<T>();
        }

        public IocManager()
        {
            IocBuilder = new ContainerBuilder();
            IocBuilder.RegisterType<SqlConnection>().Named<IDbConnection>(DatabaseType.Sql.ToString());
            IocBuilder.RegisterType<SQLiteConnection>().Named<IDbConnection>(DatabaseType.Sqlite.ToString());
        }

        public void Initialize()
        {
            IocContainer = IocBuilder.Build();
        }
    }
}
