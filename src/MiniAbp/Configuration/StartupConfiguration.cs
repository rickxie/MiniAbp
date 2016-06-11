using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Dependency;

namespace MiniAbp.Configuration
{
    public class StartupConfiguration : DictionayBasedConfig, IStartupConfiguration
    {
        public IocManager IocManager { get; private set; }
        public DatabaseConfiguration Database { get; set; }
        public AuditConfiguration Auditing { get; set; }
        public LocalizationConfiguration Localization { get; set; }
        public CustomConfiguration Custom { get; set; }

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
        }
    }
}
