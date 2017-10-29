using MiniAbp.Web.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Web.Configuration
{
    public class WebConfiguration
    {
        /// <summary>
        /// Whether proxy js camel cased.
        /// </summary>
        public bool IsProxyJsCamelCase { get; set; }
         
        /// <summary>
        /// Js Proxy Output Type
        /// </summary>
        public ProxyOutputType ProxyType { get; set; }

        /// <summary>
        /// Start with no slash example:  "Content\\Lib\\miniAbp\\auto\\";
        /// </summary>
        public string OutputRelativePath { get; set; }

        /// <summary>
        /// Website Configuration
        /// </summary>
        public WebConfiguration() {
            ProxyType = ProxyOutputType.Angular;
            OutputRelativePath = "Content\\Lib\\miniAbp\\auto\\";
        }

    }


    public enum ProxyOutputType {
        AngularJs,
        Angular
    }
}
