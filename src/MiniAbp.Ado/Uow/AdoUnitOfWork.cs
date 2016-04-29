using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MiniAbp.Dependency;
using MiniAbp.Domain;
using MiniAbp.Domain.Uow;

namespace MiniAbp.Ado.Uow
{
    public class AdoUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        private readonly IDictionary<Type, IDbContext> _activeDbContexts;
        private readonly IocManager _iocResolver;
        private IDbTransaction _transaction;

        public AdoUnitOfWork(IocManager iocManager)
        {
            _iocResolver = iocManager;
            _activeDbContexts = new Dictionary<Type, IDbContext>();
        }
        protected override void BeginUow()
        {
            _iocResolver.Resolve<IDbConnection>();
        }

        protected override void CompleteUow()
        {
            throw new NotImplementedException();
        }

        protected override Task CompleteUowAsync()
        {
            throw new NotImplementedException();
        }

        protected override void DisposeUow()
        {
            throw new NotImplementedException();
        }
        internal IDbContext GetOrCreateDbContext()
        {
            IDbContext dbContext;
            if (!_activeDbContexts.TryGetValue(typeof(IDbContext), out dbContext))
            {
                dbContext = _iocResolver.Resolve<IDbContext>();
                _activeDbContexts[typeof(IDbContext)] = dbContext;
            }

            return (IDbContext)dbContext;
        }
    }
}
