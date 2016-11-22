using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Configuration;
using MiniAbp.DataAccess;
using MiniAbp.Dependency;
using MiniAbp.Logging;

namespace MiniAbp
{
    /// <summary>
    /// Miniabp framework class
    /// </summary>
    public static class MiniAbp
    {
        public static IStartupConfiguration Configuration => IocManager.Instance.Resolve<IStartupConfiguration>();

        private static readonly YBootstrapper Boot = new YBootstrapper();
        public static void StartWithSqlServer(string connectionStringOrname)
        {
            Boot.Initialize();
            Configuration.UseSqlServerStorage(connectionStringOrname);
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, eventArgs) =>
                {
                    IocManager.Instance.Resolve<ILogger>()
                        .Error(sender.ToString() + eventArgs.ExceptionObject.ToString());
                };
        }
    }
}
