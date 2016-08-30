using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Castle.MicroKernel.Registration;
using MiniAbp.Ado.Dependency;
using MiniAbp.Ado.Uow;
using MiniAbp.Reflection;

namespace MiniAbp.Ado
{
    public class AdoModule : MabpModule
    {
        private readonly TypeFinder _typeFinder;
        public AdoModule(TypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new AdoConventionalRegisterer());
            base.PreInitialize();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            IocManager.IocContainer.Register(
                Component.For(typeof(IDbContextProvider))
                    .ImplementedBy(typeof(UnitOfWorkDbContextProvider))
                    .LifestyleTransient()
                );
            RegisterGenericRepositories();
        }

        private void RegisterGenericRepositories()
        {
            //var dbContextTypes =
            //    _typeFinder.Find(type =>
            //        type.IsPublic &&
            //        !type.IsAbstract &&
            //        type.IsClass &&
            //        typeof(AdoDbContext).IsAssignableFrom(type)
            //        );

            //if (dbContextTypes.IsNullOrEmpty())
            //{
            //    return;
            //}

//            foreach (var dbContextType in dbContextTypes)
//            {
//                EntityFrameworkGenericRepositoryRegistrar.RegisterForDbContext(dbContextType, IocManager);
//            }
        }
    }
}
