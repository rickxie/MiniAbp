using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Dependency;

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

        public UnitOfWorkManager(IocManager iocResolver, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IUnitOfWorkDefaultOptions defaultOptions)
        {
            _iocResolver = iocResolver;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _defaultOptions = defaultOptions;
        }


    }
}
