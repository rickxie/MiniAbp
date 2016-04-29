
using System;
using System.Security.Principal;
using System.Threading;
using MiniAbp.Configuration;
using MiniAbp.DataAccess;

namespace MiniAbp.Test
{
    public class TestBase
    {
        public YBootstrapper Bootstrapper;
        public TestBase()
        {
            Bootstrapper = new YBootstrapper(); 
        } 

        public void Initialize(string connectionStr, Dialect dialect)
        {
            Bootstrapper.Initialize();
            var dbSetting = Bootstrapper.IocManager.Resolve<DatabaseSetting>();
            dbSetting.ConnectionString = connectionStr;
            dbSetting.Dialect = dialect;
        }

        public void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
        }
    }
}
