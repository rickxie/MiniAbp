using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Dependency;
using System.Transactions;

namespace MiniAbp.Domain.Uow
{
    internal class  UnitOfWorkManager : IUnitOfWorkManager, ITransientDependency
    {
        private readonly IocManager _iocResolver;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly IUnitOfWorkDefaultOptions _defaultOptions;
        public IActiveUnitOfWork Current => _currentUnitOfWorkProvider.Current;
        public IUnitOfWorkCompleteHandle Begin()
        {
            return Begin(new UnitOfWorkOptions());
        }

        public IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options)
        {
            options.FillDefaultsForNonProvidedOptions(_defaultOptions);
            var outerUow = _currentUnitOfWorkProvider.Current;

            if (options.Scope == TransactionScopeOption.Required && outerUow != null)
            {
                return new InnerUnitOfWorkCompleteHandle();
            }
            var uow = _iocResolver.Resolve<IUnitOfWork>();
            uow.Completed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            uow.Failed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            uow.Disposed += (sender, args) =>
            {
                _iocResolver.Release(uow);
            };

            uow.Begin(options);

            _currentUnitOfWorkProvider.Current = uow;

            return uow;
        }
        public IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope)
        {
            return Begin(new UnitOfWorkOptions { Scope = scope });
        }
        public UnitOfWorkManager(IocManager iocResolver, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IUnitOfWorkDefaultOptions defaultOptions)
        {
            _iocResolver = iocResolver;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _defaultOptions = defaultOptions;
        }


    }
}
