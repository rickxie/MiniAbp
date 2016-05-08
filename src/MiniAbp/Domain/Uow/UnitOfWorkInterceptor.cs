using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace MiniAbp.Domain.Uow
{
    public class UnitOfWorkInterceptor: IInterceptor
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UnitOfWorkInterceptor(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void Intercept(IInvocation invocation)
        {
            if (_unitOfWorkManager.Current != null)
            {
                invocation.Proceed();
                return;
            }
            using (var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions()))
            {
                invocation.Proceed();
                uow.Complete();
            }
        }
    }
}
