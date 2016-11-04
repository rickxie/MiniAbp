using System;
using System.Web;
using MiniAbp.Authorization;
using MiniAbp.Configuration;
using MiniAbp.Dependency;
using MiniAbp.Reflection;
using MiniAbp.Web.Route;

namespace MiniAbp.Web
{
    public abstract class YApplication: HttpApplication
    {
        protected YBootstrapper Bootstrapper { get; private set; }
        protected YApplication()
        {
            Bootstrapper = new YBootstrapper();
        }

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            Bootstrapper.IocManager.RegisterIfNot<IAssemblyFinder, WebAssemblyFinder>();
            Bootstrapper.Initialize();
        }

        protected virtual void Session_Start(object sender, EventArgs e)
        {

        }

        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
        }


        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                 PrincipalHelper.SetPrincipal(Context.User);
            }
            else
            {
                 PrincipalHelper.SetPrincipal(null);
            }
            UrlRouting.Instance.HandleApiService((System.Web.HttpApplication)sender);
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {
            Bootstrapper.HandleException(Server.GetLastError().GetBaseException());
        }

        protected virtual void Session_End(object sender, EventArgs e)
        {

        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            Bootstrapper.Dispose();
        }
    }
}
