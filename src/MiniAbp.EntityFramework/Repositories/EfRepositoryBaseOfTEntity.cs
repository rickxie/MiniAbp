using System.Data.Entity;
using MiniAbp.Domain;
using MiniAbp.Domain.Uow;
using MiniAbp.Domain.Repositories;

namespace MiniAbp.EntityFramework.Repositories
{
    public class EfRepositoryBase<TDbContext, TEntity> : EfRepositoryBase<TDbContext, TEntity, string>, IRepository<TEntity>
        where TEntity : class, IEntity<string>
        where TDbContext : DbContext
    {
        public EfRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }
    }
}