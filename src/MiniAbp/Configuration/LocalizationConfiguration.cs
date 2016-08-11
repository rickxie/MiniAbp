using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Localization;

namespace MiniAbp.Configuration
{
    public class LocalizationConfiguration
    {
        /// <summary>
        /// Used to set languages available for this application.
        /// </summary>
        public List<LanguageInfo> Languages { get; }

        /// <summary>
        /// List of localization sources.
        /// </summary>
        public List<LocalizationSource> Sources { get; }

        /// <summary>
        /// Used to enable/disable localization system.
        /// Default: true.
        /// </summary>
        bool IsEnabled { get; set; }

        public LocalizationConfiguration()
        {
            Languages = new List<LanguageInfo>();
            Sources = new List<LocalizationSource>();
            IsEnabled = true;
        }
    }
}
