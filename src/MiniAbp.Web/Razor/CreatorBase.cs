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
using MiniAbp.Web.Configuration;
using MiniAbp.Domain;

// For extension methods.


namespace MiniAbp.Web.Razor
{
    public class ProxyCreatorBase: ISingletonDependency
    {
        public WebConfiguration Config { get; set; }
        public LocalizationManager LocalizationMng { get; set; }
        public ProxyCreatorBase()
        {
        }
        
        protected List<ServiceWithMethod> Build(List<Type> svTypes, List<Type> itTypes)
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

        protected string ReadTemplate(string fileName)
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
