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
        public static void Initialize(IocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += KernelOnComponentRegistered;
        }

        private static void KernelOnComponentRegistered(string key, IHandler handler)
        {
            if (UnitOfWorkHelper.IsConventionalUowClass(handler.ComponentModel.Implementation))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
            }
        } 
    }
}
