
using System;
using System.Security.Principal;
using System.Threading;

namespace MiniAbp.Test
{
    public class TestBase
    {
        public YBootstrapper Bootstrapper;
        public TestBase()
        {
            Bootstrapper = new YBootstrapper(); 
        }

        public  virtual void Initialize()
        {
            Bootstrapper.Initialize();
        }

        public void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
        }
    }
}
