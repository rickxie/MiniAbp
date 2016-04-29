using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MiniAbp.Configuration;
using MiniAbp.DataAccess;
using MiniAbp.Logging;
using MiniAbp.Modules;
using MiniAbp.Reflection;

namespace MiniAbp.Dependency.Installer
{
    internal class CoreInstaller: IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<TypeFinder>().ImplementedBy<TypeFinder>().LifestyleSingleton(),
                Component.For<ModuleFinder>().ImplementedBy<ModuleFinder>().LifestyleTransient(),
                Component.For<ModuleManager>().ImplementedBy<ModuleManager>().LifestyleSingleton(),
                Component.For<IDbConnection>().ImplementedBy<SqlConnection>().Named(Dialect.SqlServer.ToString()).LifeStyle.Transient,
                Component.For<IDbConnection>().ImplementedBy<SQLiteConnection>().Named(Dialect.SqLite.ToString()).LifeStyle.Transient,
                Component.For<ILogger>().ImplementedBy<FileLogger>().LifeStyle.Transient,
                Component.For<IStartupConfiguration>().ImplementedBy<StartupConfiguration>().LifestyleSingleton(),
                Component.For<DatabaseSetting>().ImplementedBy<DatabaseSetting>().LifestyleSingleton());
        }
    }
}
