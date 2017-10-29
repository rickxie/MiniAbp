using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Reflection;
using Castle.MicroKernel.Registration;
using MiniAbp.EntityFramework.Uow;
using System.Reflection;
using Abp.EntityFramework.Uow;
using MiniAbp.EntityFramework.Common;
using MiniAbp.EntityFramework.Repositories;
using Castle.Core.Internal;
using MiniAbp.Dependency;
using MiniAbp.Orm;
using System.Data.Entity.Infrastructure.Interception;

namespace MiniAbp.EntityFramework
{
    /// <summary>
    /// version of entityframework dataaccess layer
    /// </summary>
    [DependsOn(typeof(KernelModule))]
    public class MiniAbpEntityFrameworkModule : MabpModule
    {
        private static WithNoLockInterceptor _withNoLockInterceptor;
        private readonly ITypeFinder _typeFinder;

        private static readonly object WithNoLockInterceptorSyncObj = new object();
        public MiniAbpEntityFrameworkModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            //Configuration.ReplaceService<IUnitOfWorkFilterExecuter>(() =>
            //{
            //    IocManager.IocContainer.Register(
            //        Component
            //        .For<IUnitOfWorkFilterExecuter, IEfUnitOfWorkFilterExecuter>()
            //        .ImplementedBy<EfDynamicFiltersUnitOfWorkFilterExecuter>()
            //        .LifestyleTransient()
            //    );
            //});
        }
        public override void Initialize()
        {

            IocManager.IocContainer.Register(Component.For(typeof(IDbContextTypeMatcher)).ImplementedBy(typeof(DbContextTypeMatcher)).LifestyleTransient());


            //if (!Configuration.UnitOfWork.IsTransactionScopeAvailable)
            //{
                IocManager.RegisterIfNot<IEfTransactionStrategy, DbContextEfTransactionStrategy>(DependencyLifeStyle.Transient);
            //}

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            IocManager.IocContainer.Register(
                Component.For(typeof(IDbContextProvider<>))
                    .ImplementedBy(typeof(UnitOfWorkDbContextProvider<>))
                    .LifestyleTransient()
                );

            RegisterGenericRepositoriesAndMatchDbContexes();
            RegisterWithNoLockInterceptor();
        }
        private void RegisterWithNoLockInterceptor()
        {
            lock (WithNoLockInterceptorSyncObj)
            {
                if (_withNoLockInterceptor != null)
                {
                    return;
                }

                _withNoLockInterceptor = IocManager.Resolve<WithNoLockInterceptor>();
                DbInterception.Add(_withNoLockInterceptor);
            }
        }
        private void RegisterGenericRepositoriesAndMatchDbContexes()
        {
            var dbContextTypes =
                _typeFinder.Find(type =>
                    type.IsPublic &&
                    !type.IsAbstract &&
                    type.IsClass &&
                    typeof(EfDbContext).IsAssignableFrom(type)
                    );

            if (dbContextTypes.IsNullOrEmpty())
            {
                Logger.Warn("No class found derived from AbpDbContext.");
                return;
            }

            using (var scope = IocManager.CreateScope())
            {
                var repositoryRegistrar = scope.Resolve<IEfGenericRepositoryRegistrar>();

                foreach (var dbContextType in dbContextTypes)
                {
                    Logger.Debug("Registering DbContext: " + dbContextType.AssemblyQualifiedName);
                    repositoryRegistrar.RegisterForDbContext(dbContextType, IocManager, EfAutoRepositoryTypes.Default);

                    IocManager.IocContainer.Register(
                        Component.For<ISecondaryOrmRegistrar>()
                            .Named(Guid.NewGuid().ToString("N"))
                            .Instance(new EfBasedSecondaryOrmRegistrar(dbContextType, scope.Resolve<IDbContextEntityFinder>()))
                            .LifestyleTransient()
                    );
                }

                scope.Resolve<IDbContextTypeMatcher>().Populate(dbContextTypes);
            }
        }
    }
}
