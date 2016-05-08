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
        private IDbContext _dbContext => GetOrCreateDbContext();
        private IDbConnection dbConnection => _dbContext.DbConnection;
        private IDbTransaction dbTransaction;
        public AdoUnitOfWork(IocManager iocManager)
        {
            _iocResolver = iocManager;
            _activeDbContexts = new Dictionary<Type, IDbContext>();
        }
        protected override void BeginUow()
        {
            dbConnection?.Open();
            if(_dbContext!=null)
            _dbContext.DbTransaction = dbTransaction = dbConnection?.BeginTransaction();
        }

        protected override void CompleteUow()
        {
            dbTransaction?.Commit();
            dbConnection?.Close();
        }

        protected override void OnFailed(Exception exception)
        {
            dbTransaction?.Rollback();
        }

        protected override void DisposeUow()
        {
            dbTransaction?.Dispose();
            dbConnection?.Dispose();
        }

        internal IDbContext GetOrCreateDbContext()
        {
            IDbContext dbContext;
            if (!_activeDbContexts.TryGetValue(typeof(IDbContext), out dbContext))
            {
                dbContext = _iocResolver.Resolve<AdoDbContext>();
                _activeDbContexts[typeof(IDbContext)] = dbContext;
            }

            return (IDbContext)dbContext;
        }
    }
}
