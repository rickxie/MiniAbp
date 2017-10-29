using MiniAbp.Runtime;
using MiniAbp.Web.Configuration;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Web.Razor
{
    public class AngularProxyCreator : ProxyCreatorBase, IProxyCreator
    {
        public void CreateProxy(List<Type> serviceType, List<Type> serviceInterface)
        {
            CreateJsProxy(serviceType, serviceInterface);
            CreateLocalizationProxy();
        }
        public void CreateJsProxy(List<Type> svTypes, List<Type> itTypes)
        {
            if (svTypes == null || svTypes.Count == 0)
                return;
            var config = new TemplateServiceConfiguration { EncodedStringFactory = new RawStringFactory() };
            var service = RazorEngineService.Create(config);
            Engine.Razor = service;
            var template = ReadTemplate("Angular_Proxy_tmp.cshtml");
            var model = Build(svTypes, itTypes);
            var result = Engine.Razor.RunCompile(template, "Services", typeof(List<ServiceWithMethod>), model);
            var savePath = AppPath.GetFullDirByRelativeDir(Config.OutputRelativePath);
            File.WriteAllText(savePath + "mabp.service.ts", result, System.Text.Encoding.UTF8);
        }
        public void CreateLocalizationProxy()
        {
            var template = ReadTemplate("Localization_Proxy_tmp.cshtml");
            var result = Engine.Razor.RunCompile(template, "Localization", typeof(Dictionary<string, Dictionary<string, string>>), LocalizationMng.Sources);
            var savePath = AppPath.GetFullDirByRelativeDir(Config.OutputRelativePath);
            File.WriteAllText(savePath + "localization.js", result, System.Text.Encoding.UTF8);
        }
    }
}
