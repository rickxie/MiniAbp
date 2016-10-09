using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Extension;

namespace MiniAbp.Localization
{
    /// <summary>
    /// 本地化源
    /// </summary>
    public class LocalizationSource
    {
        public string Source { get; set; }
        public ILocalizationProvider Provider { get; set; }
        public LocalizationSource(string source, ILocalizationProvider provider)
        {
            this.Source = source;
            Provider = provider;
        }
    }
}
