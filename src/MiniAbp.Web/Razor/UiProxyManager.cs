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
using MiniAbp.Dependency;

// For extension methods.


namespace MiniAbp.Web.Razor
{
    public class UiProxyManager
    {
        public WebConfiguration Config { get; set; }
        public IIocManager IocResover { get; set; }
        public IProxyCreator Creator { get; set; } 
        public UiProxyManager(WebConfiguration config, IIocManager iocManager)
        {
            Config = config;
            IocResover = iocManager;
        }
        /// <summary>
        /// Create proxy js files
        /// </summary>
        /// <param name="svTypes"></param>
        /// <param name="itTypes"></param>
        public void GenerateProxy(List<Type> svTypes, List<Type> itTypes )
        {
            switch (Config.ProxyType)
            {
                case ProxyOutputType.AngularJs:
                    Creator = IocResover.Resolve<AngularJsProxyCreator>();
                    break;
                case ProxyOutputType.Angular:
                    Creator = IocResover.Resolve<AngularProxyCreator>();
                    break;
                default:
                    break;
            }
            Creator.CreateProxy(svTypes, itTypes);
        }        
    }
}
