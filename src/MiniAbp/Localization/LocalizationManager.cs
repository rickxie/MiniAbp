using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using MiniAbp.Configuration;
using MiniAbp.Extension;

namespace MiniAbp.Localization
{
    public class LocalizationManager
    {
        public readonly LocalizationConfiguration Config;

        /// <summary>
        /// <para>first string is the [resourceName.languageName]</para>
        /// <para>second string is the [language.Key] [language.Value] pair</para>
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Sources { get; set; }

        public LocalizationManager(LocalizationConfiguration config)
        {
            Config = config;
            Sources = new Dictionary<string, Dictionary<string, string>>();
        }

        public Dictionary<string, string> GetResouce(string source, string lang)
        {
            var dicKey = source + "." + lang;
            if (!Sources.ContainsKey(dicKey))
            {
                throw new NullReferenceException("资源'{0}'或其对应的多语言文件'{1}'不存在".Fill(source, lang));
            }
            return Sources[dicKey];
        }

        //加载至全局
        public void Initialize()
        {
            Config.Sources.ForEach(r =>
            {
                r.Provider.Load(r, Config.Languages, Sources);
            });
        }
    }
}
