using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using MiniAbp.Dependency;

namespace MiniAbp.Web.Mvc
{
    /// <summary>
    /// Registers all MVC Controllers derived from <see cref="Controller"/>.
    /// </summary>
    public class ControllerConventionalRegistrar : IConventionalDependencyRegistrar
    {
        /// <inheritdoc/>
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .BasedOn<Controller>()
                    .LifestyleTransient()
                );
        }
    }
}