using System.Data.Entity;
using MiniAbp.Dependency;
using MiniAbp.Domain.Uow;
using MiniAbp.EntityFramework.Common;

namespace Abp.EntityFramework.Uow
{
    public interface IEfTransactionStrategy
    {
        void InitOptions(UnitOfWorkOptions options);

        DbContext CreateDbContext<TDbContext>(string connectionString, IDbContextResolver dbContextResolver)
            where TDbContext : DbContext;

        void Commit();

        void Dispose(IocManager iocResolver);
    }
}