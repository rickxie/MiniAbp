using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using MiniAbp.Domain.Entitys;
using MiniAbp.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;

namespace MiniAbp.Localization
{
    /// <summary>
    /// Used for localization configurations.
    /// </summary>
    internal class LocalizationProvider
    {
        public LocalizationProvider()
        {
           
        }

        public void Load(LocalizationSource source, List<LanguageInfo> language, Dictionary<string, Dictionary<string,string>>  sourceDict)
        {
            if (sourceDict.ContainsKey(source.Source))
            {
                throw new ArgumentException("Source name {0} is duplicate".Fill(source.Source));
            }
            //Source
            //AllLanguage
            foreach (var languageInfo in language)
            {
                var langDic = new Dictionary<string, string>();
                var filePath = "{0}\\{1}.{2}.json".Fill(source.Path, source.Source, languageInfo.Name);
                if (!File.Exists(filePath))
                {
                    throw new Exception(filePath + " is not exists.");
                }
                else
                {
                    var langStr = File.ReadAllText(filePath);
                    var json = JsonConvert.DeserializeObject<List<NameValue>>(langStr,
                        new JsonSerializerSettings() {ContractResolver = new CamelCasePropertyNamesContractResolver()});
                    for (int i = 0; i < json.Count; i++)
                    {
                        if (langDic.ContainsKey(json[i].Name))
                        {
                            throw new Exception("name '{0}' is duplicate. at {1}".Fill(json[i].Name, filePath));
                        }
                        langDic.Add(json[i].Name, json[i].Value);
                    }
                }
                sourceDict.Add(source.Source +"."+ languageInfo.Name, langDic);
            }
        }
    }
}
