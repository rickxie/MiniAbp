using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Localization
{
    /// <summary>
    /// provider the explaination for source file.
    /// </summary>
    public interface ILocalizationProvider
    {
        /// <summary>
        /// 加载最新代码
        /// </summary>
        /// <param name="source"></param>
        /// <param name="language"></param>
        /// <param name="sourceDict"></param>
        void Load(LocalizationSource source, List<LanguageInfo> language,
            Dictionary<string, Dictionary<string, string>> sourceDict);
    }
}
