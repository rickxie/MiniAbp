using System.Security.Principal;
using System.Threading;
using MiniAbp.Configuration;
using MiniAbp.DataAccess;
using MiniAbp.Dependency;
using MiniAbp.Reflection;
using MiniAbp.Web;

namespace MiniAbp.Test
{
    public class TestBase
    {
        public YBootstrapper Bootstrapper;
        public TestBase()
        {
            Bootstrapper = new YBootstrapper(); 
        }

        public virtual void Initialize(string connectionStr, Dialect dialect)
        {
            //Bootstrapper.IocManager.RegisterIfNot<IAssemblyFinder, WebAssemblyFinder>();
            PreInitialize();
            Bootstrapper.Initialize();
            var dbSetting = Bootstrapper.IocManager.Resolve<DatabaseSetting>();
            dbSetting.ConnectionString = connectionStr;
            dbSetting.Dialect = dialect;
        }

        public virtual void PreInitialize()
        {
            
        }
        public void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
        }
    }
}
