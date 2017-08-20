using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using MiniAbp.Dependency;

namespace MiniAbp.Domain.Uow
{
    public class UnitOfWorkRegistrar
    {
        //public static void Initialize(IocManager iocManager)
        //{
        //    iocManager.IocContainer.Kernel.ComponentRegistered += KernelOnComponentRegistered;
        //}

        //private static void KernelOnComponentRegistered(string key, IHandler handler)
        //{
        //    if (UnitOfWorkHelper.IsConventionalUowClass(handler.ComponentModel.Implementation))
        //    {
        //        handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
        //    }
        //}




        /// <summary>
        /// Initializes the registerer.
        /// </summary>
        /// <param name="iocManager">IOC manager</param>
        public static void Initialize(IocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                var implementationType = handler.ComponentModel.Implementation.GetTypeInfo();

                HandleTypesWithUnitOfWorkAttribute(implementationType, handler);
                HandleConventionalUnitOfWorkTypes(iocManager, implementationType, handler);
            };
        }

        private static void HandleTypesWithUnitOfWorkAttribute(TypeInfo implementationType, IHandler handler)
        {
            if (HasUnitOfWorkAttribute(implementationType) || AnyMethodHasUnitOfWork(implementationType))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
            }
        }

        private static void HandleConventionalUnitOfWorkTypes(IocManager iocManager, TypeInfo implementationType, IHandler handler)
        {
            if (!iocManager.IsRegistered<IUnitOfWorkDefaultOptions>())
            {
                return;
            }

            var uowOptions = iocManager.Resolve<IUnitOfWorkDefaultOptions>();

            if (uowOptions.IsConventionalUowClass(implementationType.AsType()))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
            }
        }
         

        private static bool AnyMethodHasUnitOfWork(TypeInfo implementationType)
        {
            return implementationType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(HasUnitOfWorkAttribute);
        }


        private static bool HasUnitOfWorkAttribute(MemberInfo implementationType) {
            return implementationType.IsDefined(typeof(UnitOfWorkAttribute), true);
        }

    }
}
