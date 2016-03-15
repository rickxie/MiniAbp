using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using MiniAbp.Domain;
using MiniAbp.Extension;
using MiniAbp.Runtime;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;
// For extension methods.


namespace MiniAbp.Razor
{
    public static class TemplateManager
    {
        public static void GenerateCode(Type[] types)
        {
            if (types == null || types.Length == 0)
               return;
            var config = new TemplateServiceConfiguration {EncodedStringFactory = new RawStringFactory()};
            var service = RazorEngineService.Create(config);
            Engine.Razor = service;
            var template = ReadTemplate("ServicesTemplate.cshtml");
            var result = Engine.Razor.RunCompile(template, "Services", typeof(Type[]), types.ToArray());
            var savePath = AppPath.GetRelativeDir("Content\\Lib\\miniAbp\\auto\\");
            File.WriteAllText(savePath + "mabpProxy.js", result);
        }

        private static string ReadTemplate(string fileName)
        {
            var resourceFile = "MiniAbp.Razor.Template.{0}".Fill(fileName);
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
