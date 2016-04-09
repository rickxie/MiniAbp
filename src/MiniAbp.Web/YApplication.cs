using System;
using System.Web;
using MiniAbp.Authorization;
using MiniAbp.Web.Route;

namespace MiniAbp.Web
{
    public abstract class YApplication: HttpApplication
    {
        protected YBootstrapper BootStraper { get; private set; }
        protected YApplication()
        {
            BootStraper = new YBootstrapper();
        }

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            BootStraper.Initialize();
            BootStraper.PostInitialize();
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
            UrlRouting.Instance.HandleApiService(((System.Web.HttpApplication)sender),
               (response, outputJson) =>
               {
                   response.ContentType = "application/json; charset=utf-8";
                   response.Write(outputJson);
                   response.End();
               });
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {
            BootStraper.HandleException(Server.GetLastError().GetBaseException());
        }

        protected virtual void Session_End(object sender, EventArgs e)
        {

        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
        }
    }
}
