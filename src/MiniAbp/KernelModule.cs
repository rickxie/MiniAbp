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
using MiniAbp.Runtime.Remoting;
using System.IO;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using MiniAbp.Extension;

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
            IocManager.Register<IScopedIocResolver, ScopedIocResolver>(DependencyLifeStyle.Transient);
            IocManager.Register(typeof(IAmbientScopeProvider<>), typeof(DataContextAmbientScopeProvider<>), DependencyLifeStyle.Transient);

            UnitOfWorkRegistrar.Initialize(IocManager);
            AuthorizationInterceptorRegistrar.Initialize(IocManager);
            AddIgnoredTypes();
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
        private void AddIgnoredTypes()
        {
            var commonIgnoredTypes = new[]
            {
                typeof(Stream),
                typeof(Expression)
            };

            foreach (var ignoredType in commonIgnoredTypes)
            {
                Configuration.Auditing.IgnoredTypes.AddIfNotContains(ignoredType);
                //Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }

            var validationIgnoredTypes = new[] { typeof(Type) };
            foreach (var ignoredType in validationIgnoredTypes)
            {
                //Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }
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
