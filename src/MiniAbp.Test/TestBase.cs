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

        public virtual void Initialize()
        {
            PreInitialize();
            Bootstrapper.Initialize();
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
