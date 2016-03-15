using System;
using System.Diagnostics;
using System.Threading;
using System.Web;
using MiniAbp.Authorization;
using Yooya.Bpm.Framework;

namespace MiniAbp
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
            BootStraper.RegisterRoute(sender);
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
