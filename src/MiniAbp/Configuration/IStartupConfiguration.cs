using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Dependency;

namespace MiniAbp.Configuration
{
    public interface IStartupConfiguration : IDictionaryBasedConfig
    {
        /// <summary>
        /// Gets the IOC manager associated with this configuration.
        /// </summary>
        IocManager IocManager { get; }
        DatabaseConfiguration Database {get;}
        AuditConfiguration Auditing {get;}
        LocalizationConfiguration Localization { get;}
        void Initialize();

    }
}
