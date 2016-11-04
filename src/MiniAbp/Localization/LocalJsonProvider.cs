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
    public class LocalJsonProvider : ILocalizationProvider
    {
        private string Path { get; set; }
        public LocalJsonProvider(string path)
        {
            Path = path;
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
                var filePath = "{0}\\{1}_{2}.json".Fill(Path, source.Source, languageInfo.Name);
                if (!File.Exists(filePath))
                {
                    throw new Exception(filePath + " is not exists.");
                }
                else
                {
                    var langStr = File.ReadAllText(filePath);
                    List<NameValue> json;
                    try
                    {
                        json = JsonConvert.DeserializeObject<List<NameValue>>(langStr,
                            new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Localization file load failed, please check the format. "+ filePath + ex.Message + ex.StackTrace);
                    }
                    foreach (NameValue t in json)
                    {
                        if (langDic.ContainsKey(t.Name))
                        {
                            throw new Exception("name '{0}' is duplicate. at {1}".Fill(t.Name, filePath));
                        }
                        langDic.Add(t.Name, t.Value);
                    }
                }
                sourceDict.Add(source.Source +"."+ languageInfo.Name, langDic);
            }
        }
    }
}
