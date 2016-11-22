using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MiniAbp.Dependency;
using MiniAbp.Runtime;

namespace MiniAbp.Web.Mvc
{
    public static class BaseWebPage 
    {
        /// <summary>
        /// get current user's language
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string GetUserLanguage(this HttpSessionStateBase session)
        {
            return IocManager.Instance.Resolve<ISession>().LanguageCulture;
        }
        /// <summary>
        /// Get current user Id
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string GetUserId(this HttpSessionStateBase session)
        {
            return IocManager.Instance.Resolve<ISession>().UserId;
        }
    }
}
