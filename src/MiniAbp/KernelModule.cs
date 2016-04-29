using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using MiniAbp.Dependency;
using MiniAbp.Domain;
using MiniAbp.Reflection;

namespace MiniAbp
{
    /// <summary>
    /// Core moduel of Mabp system
    /// </summary>
    public class KernelModule : MabpModule
    {
        public override void PreInitialize()
        {
            //TODO: 注册UnitOfWork功能
            //TODO: 注册授权验证功能
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());
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
            base.PostInitialize();
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(),
              new ConventionalRegistrationConfig
              {
                  InstallInstallers = false
              });
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }
    }
}
