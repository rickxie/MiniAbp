using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using Castle.MicroKernel.Registration;
using MiniAbp.Configuration;
using MiniAbp.DataAccess;
using MiniAbp.Dependency;
using MiniAbp.Domain;
using MiniAbp.Runtime;

namespace MiniAbp.Compile
{
    /// <summary>
    /// Dynamic Compile Base
    /// </summary>
    public abstract class RuntimeBase: MarshalByRefObject, IRemoteInterface, ISingletonDependency
    {
        private readonly object _thisLock = new object();

        public object Invoke(string strMethod, object[] parameters)
        {
            return this.GetType().InvokeMember(strMethod, BindingFlags.InvokeMethod, null, this, parameters);
        }
        public void Initialize(string connectionString, Dialect dialect)
        {
            lock (_thisLock)
            {
                if (!IocManager.Instance.IsRegistered<DatabaseConfiguration>())
                    IocManager.Instance.Register<DatabaseConfiguration>();
                if (!IocManager.Instance.IsRegistered(typeof (IDbConnection)))
                {
                    //只支持Sql server
                    IocManager.Instance.IocContainer.Register(
                        Component.For<IDbConnection>()
                            .ImplementedBy<SqlConnection>()
                            .Named(Dialect.SqlServer.ToString())
                            .LifeStyle.Transient
                        );
                }
                if (!IocManager.Instance.IsRegistered(typeof (ISession)))
                {
                    //只支持Sql server
                    IocManager.Instance.IocContainer.Register(
                        Component.For<ISession>()
                            .ImplementedBy<ClaimsSession>()
                            .LifeStyle.Transient
                        );
                }
            }
            var dbSetting = IocManager.Instance.Resolve<DatabaseConfiguration>();
            dbSetting.ConnectionString = connectionString;
            dbSetting.Dialect = dialect;
        }

    }
}
