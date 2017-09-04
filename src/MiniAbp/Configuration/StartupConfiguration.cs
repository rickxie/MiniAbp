using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MiniAbp.DataAccess;
using MiniAbp.Dependency;
using MiniAbp.Extension;
using MiniAbp.Domain.Uow;

namespace MiniAbp.Configuration
{
    public class StartupConfiguration : DictionayBasedConfig, IStartupConfiguration
    {
        public IocManager IocManager { get; private set; }
        public DatabaseConfiguration Database { get; set; }
        public AuditConfiguration Auditing { get; set; }
        public LocalizationConfiguration Localization { get; set; }
        public CustomConfiguration Custom { get; set; }
        public IUnitOfWorkDefaultOptions UnitOfWork { get; set; }
        /// <summary>
        /// Ignored types for serialization on audit logging.
        /// </summary>
        public List<Type> IgnoredTypes { get; }

        public StartupConfiguration(IocManager iocManager)
        {
            IocManager = iocManager;
        }

        public void Initialize()
        {
            Database = IocManager.Resolve<DatabaseConfiguration>();
            Localization = IocManager.Resolve<LocalizationConfiguration>();
            Auditing = IocManager.Resolve<AuditConfiguration>();
            Custom = IocManager.Resolve<CustomConfiguration>();
            UnitOfWork = IocManager.Resolve<IUnitOfWorkDefaultOptions>();
        }

        /// <summary>
        /// Use sql server as data storage provider
        /// </summary>
        /// <param name="connectStringOrName"></param>
        public void UseSqlServerStorage(string connectStringOrName)
        {
            if (connectStringOrName.IsEmpty())
            {
                throw new Exception("Connection string or Default Name can not be empty, reference: " + connectStringOrName);
            }
            else if (connectStringOrName.Length < 2)
            {
                throw new Exception("Connection string or Default Name is wrong, reference: " + connectStringOrName);
            }
            //Connection String Regex
            var reg = new Regex(@"\W");
            var result = reg.Match(connectStringOrName);
            string connStr;
            //is conn str
            if (result.Captures.Count < 1)
                connStr = ConfigurationManager.ConnectionStrings[connectStringOrName]?.ConnectionString;
            else
            {
                connStr = connectStringOrName;
            }
            Database.ConnectionString = connStr;
            Database.Dialect = Dialect.SqlServer;
        }


    }
}
