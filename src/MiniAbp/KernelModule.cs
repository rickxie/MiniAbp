using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using MiniAbp.Authorization.Interceptors;
using MiniAbp.Dependency;
using MiniAbp.Domain;
using MiniAbp.Domain.Uow;
using MiniAbp.Localization;
using MiniAbp.Reflection;
using MiniAbp.Runtime;

namespace MiniAbp
{
    /// <summary>
    /// Core moduel of Mabp system
    /// </summary>
    public class KernelModule : MabpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());
            UnitOfWorkRegistrar.Initialize(IocManager);
            AuthorizationInterceptorRegistrar.Initialize(IocManager);
            base.PreInitialize();

        }
        /// <summary>
        /// Regist self
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(),
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });
        }

        public override void PostInitialize()
        {
            //            base.PostInitialize();
            //            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(),
            //              new ConventionalRegistrationConfig
            //              {
            //                  InstallInstallers = false
            //              });
            IocManager.RegisterIfNot<ISession, ClaimsSession>(DependencyLifeStyle.Singleton);

            IocManager.Resolve<LocalizationManager>().Initialize();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }
    }
}
