using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using MiniAbp.Localization;
using MiniAbp.Reflection;
using MiniAbp.Web.Mvc;
using MiniAbp.Web.Razor;

namespace MiniAbp.Web
{
    public class WebModule : MabpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ControllerConventionalRegistrar());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IocManager.IocContainer.Kernel));
        }

        public override void PostInitialize()
        {
            var serviceTypes = YAssembly.ServiceDic.Select(r => r.Key).ToList();
            var serviceInterface = YAssembly.ServiceDic.Select(r => r.Value).ToList();
            var resouce = IocManager.Resolve<LocalizationManager>();
            TemplateManager.GenerateProxyJs(serviceTypes, serviceInterface);
            TemplateManager.GenerateLocalization(resouce);
        }
    }
}
