using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using MiniAbp.Localization;
using MiniAbp.Reflection;
using MiniAbp.Web.Mvc;
using MiniAbp.Web.Razor;
using Castle.MicroKernel.Registration;
using MiniAbp.Web.Configuration;

namespace MiniAbp.Web
{
    public class WebModule : MabpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ControllerConventionalRegistrar());
            IocManager.IocContainer.Register( Component.For<WebConfiguration>().ImplementedBy<WebConfiguration>().LifestyleSingleton());
            IocManager.IocContainer.Register(Component.For<UiProxyManager>().ImplementedBy<UiProxyManager>().LifestyleSingleton());

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IocManager.IocContainer.Kernel));

            Configuration.UnitOfWork.ConventionalUowSelectors.Add(type => type.IsAssignableFrom(typeof(IController)));
        }

        public override void PostInitialize()
        {
            var serviceTypes = YAssembly.ServiceDic.Select(r => r.Key).ToList();
            var serviceInterface = YAssembly.ServiceDic.Select(r => r.Value).ToList();
            var proxyManager = IocManager.Resolve<UiProxyManager>();
            proxyManager.GenerateProxy(serviceTypes, serviceInterface);
        }
    }
}
