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
        public DatabaseSetting DbSetting { get; set; }

        public StartupConfiguration(IocManager iocManager)
        {
            IocManager = iocManager;
        }

        public void Initialize()
        {
            this.DbSetting = IocManager.Resolve<DatabaseSetting>();
        }
    }
}
