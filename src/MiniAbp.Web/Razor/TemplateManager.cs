using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniAbp.Extension;
using MiniAbp.Localization;
using MiniAbp.Runtime;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;
using Encoding = System.Text.Encoding;

// For extension methods.


namespace MiniAbp.Web.Razor
{
    public static class TemplateManager
    {
        public static void GenerateProxyJs(List<Type> svTypes, List<Type> itTypes )
        {
            if (svTypes == null || svTypes.Count == 0)
               return;
            var config = new TemplateServiceConfiguration {EncodedStringFactory = new RawStringFactory()};
            var service = RazorEngineService.Create(config);
            Engine.Razor = service;
            var template = ReadTemplate("ServicesTemplate.cshtml");
            var model = Build(svTypes, itTypes);
            var result = Engine.Razor.RunCompile(template, "Services", typeof(List<ServiceWithMethod>), model);
            var savePath = AppPath.GetRelativeDir("Content\\Lib\\miniAbp\\auto\\");
            File.WriteAllText(savePath + "mabpProxy.js", result, Encoding.UTF8);
        }

        public static void GenerateLocalization(LocalizationManager localizationManager)
        {
            var template = ReadTemplate("LocalizationTemplate.cshtml");
            var result = Engine.Razor.RunCompile(template, "Localization", typeof(Dictionary<string, Dictionary<string, string>>), localizationManager.Sources);
            var savePath = AppPath.GetRelativeDir("Content\\Lib\\miniAbp\\auto\\");
            File.WriteAllText(savePath + "localization.js", result, Encoding.UTF8);
        }
        private static List<ServiceWithMethod> Build(List<Type> svTypes, List<Type> itTypes)
        {
            var result = new List<ServiceWithMethod>();

            for (var i = 0; i < svTypes.Count; i ++)
            {
                var item = svTypes[i];
                var serviceName = item.Name;
                if (serviceName.ToUpper().EndsWith("SERVICE"))
                {
                    serviceName = serviceName.Substring(0, serviceName.Length - 7);
                }
                else if (serviceName.ToUpper().EndsWith("SV"))
                {
                    serviceName = serviceName.Substring(0, serviceName.Length - 2);
                }
                var methods =
                    itTypes[i].GetMethods()
                        .Where(
                            r =>
                                !r.IsSpecialName && !r.IsStatic && r.IsPublic && r.IsSecurityCritical &&
                                !r.IsSecuritySafeCritical)
                        .ToList();
                var names = methods.Select(r => r.Name).ToList();
                result.Add(new ServiceWithMethod() {MethodNames = names, ServiceName = serviceName});
            }
            return result;
        }

        private static string ReadTemplate(string fileName)
        {
            var resourceFile = "MiniAbp.Web.Razor.Template.{0}".Fill(fileName);
            System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceFile);
            string tplStr = string.Empty;
            if (stream != null)
                using (System.IO.StreamReader sr = new System.IO.StreamReader(stream))
                {
                    tplStr = sr.ReadToEnd();
                }
            return tplStr;
        }
    }
}
