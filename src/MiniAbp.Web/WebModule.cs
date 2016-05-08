using System.Reflection;
using System.Web.Mvc;
using MiniAbp.Reflection;
using MiniAbp.Web.Mvc;

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
    }
}
