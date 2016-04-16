using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MiniAbp.DataAccess;
using MiniAbp.Domain;
using MiniAbp.Logging;
using MiniAbp.Reflection;

namespace MiniAbp.Dependency
{
    public class IocManager 
    {
        public static IocManager Instance { get; private set; }
        private WindsorContainer IocContainer { get; set; }
        static IocManager()
        {
            Instance = new IocManager();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="param">匿名类型参数</param>
        /// <returns></returns>
        public T ResolveNamed<T>(string serviceName, object param)
        {
            return IocContainer.Resolve<T>(serviceName, param);
        }

        public T Resolve<T>()
        {
            return IocContainer.Resolve<T>();
        }

        public IocManager()
        {
            IocContainer = new WindsorContainer();
            IocContainer.Register(
                Component.For<IDbConnection>().ImplementedBy<SqlConnection>().Named(Dialect.SqlServer.ToString()).LifeStyle.Transient);
            IocContainer.Register(
                Component.For<IDbConnection>().ImplementedBy<SQLiteConnection>().Named(Dialect.SqLite.ToString()).LifeStyle.Transient);
            IocContainer.Register(Component.For<ILogger>().ImplementedBy<FileLogger>().LifeStyle.Transient);
        }

        public void Initialize()
        {
            YAssembly.Initialize();
            IocContainer.Register(Classes.From(YAssembly.ServiceTypes).BasedOn<BaseService>().LifestyleTransient());
            IocContainer.Register(Classes.From(YAssembly.RepositoryTypes).BasedOn<BaseService>().LifestyleTransient());
        }
    }
}
