using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Extension;

namespace MiniAbp.Localization
{
    public class LocalizationManager
    {
        public List<LanguageInfo> Languages { get; set; } 
        public List<LocalizationSource> LocalizationSources { get; set; }

        /// <summary>
        /// <para>first string is the [resourceName.languageName]</para>
        /// <para>second string is the [language.Key] [language.Value] pair</para>
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> Sources { get; set; }

        public LocalizationManager()
        {

        }
        //初始化配置
        public void Add(List<LanguageInfo> languages, List<LocalizationSource> localizationSources)
        {
            Languages = languages;
            LocalizationSources = localizationSources;
        }

        public string Get(string source, string lang, string key)
        {
            var dicKey = source + "." + lang;
            if (!Sources.ContainsKey(dicKey))
            {
                throw new NullReferenceException("资源'{0}'或其对应的多语言文件'{1}'不存在".Fill(source, lang));
            }
            if (Sources[dicKey].ContainsKey(key))
            {
                throw new NullReferenceException("资源{0}的{1}中缺少键为{3}的定义".Fill(source, lang, key));
            }
            return Sources[dicKey][key];
        }

        //加载至全局
        public void Initialize()
        {
            var provider = new LocalizationProvider();
            LocalizationSources.ForEach(r =>
            {
                provider.Load(r, Languages, Sources);
            });
        }
    }
}
