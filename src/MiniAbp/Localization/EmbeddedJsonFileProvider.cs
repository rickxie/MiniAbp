using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Castle.Core.Internal;
using MiniAbp.Domain.Entitys;
using MiniAbp.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MiniAbp.Localization
{
    public class EmbeddedJsonFileProvider : ILocalizationProvider
    {
        public string EmbededNameSpace { get; set; }
        private readonly Assembly _embeddedAssembly;
        private readonly string _folderName;
        public EmbeddedJsonFileProvider( Assembly embeddedAssembly, string folderName)
        {
            _embeddedAssembly = embeddedAssembly;
            this._folderName = folderName;
        }
        /// <summary>
        /// Load Data
        /// </summary>
        /// <param name="source"></param>
        /// <param name="language"></param>
        /// <param name="sourceDict"></param>
        public void Load(LocalizationSource source, List<LanguageInfo> language,
            Dictionary<string, Dictionary<string, string>> sourceDict)
        {
            if (sourceDict.ContainsKey(source.Source))
            {
                throw new ArgumentException("Source name {0} is duplicate".Fill(source.Source));
            }
            //AllLanguage
            foreach (var languageInfo in language)
            {
                var langDic = new Dictionary<string, string>();
                var fileName = "{0}_{1}.json".Fill(source.Source, languageInfo.Name);

                var langStr = GetEmbeddedString(fileName);
                List<NameValue> json;
                try
                {
                    json = JsonConvert.DeserializeObject<List<NameValue>>(langStr,
                        new JsonSerializerSettings() {ContractResolver = new CamelCasePropertyNamesContractResolver()});
                }
                catch (Exception ex)
                {
                    throw new Exception("Localization file load failed, please check the format. " + fileName +
                                        ex.Message + ex.StackTrace);
                }
                foreach (NameValue t in json)
                {
                    if (langDic.ContainsKey(t.Name))
                    {
                        throw new Exception("name '{0}' is duplicate. at {1}".Fill(t.Name, fileName));
                    }
                    langDic.Add(t.Name, t.Value);
                }
                sourceDict.Add(source.Source + "." + languageInfo.Name, langDic);
            }
        }

        /// <summary>
        /// 获取嵌入的文件内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetEmbeddedString(string fileName)
        {
            //获取数据库公共方法 获得默认名称空间
            string str = _embeddedAssembly.GetName().Name + ".{0}.{1}".Fill(_folderName, fileName);
            System.IO.Stream stream = _embeddedAssembly.GetManifestResourceStream(str);
            string sql = string.Empty;
            if (stream != null)
                using (System.IO.StreamReader sr = new System.IO.StreamReader(stream))
                {
                    sql = sr.ReadToEnd();
                }
            return sql;
        }
    }
}
