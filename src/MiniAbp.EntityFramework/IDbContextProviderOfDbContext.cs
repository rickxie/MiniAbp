using System.Data.Entity;
using MiniAbp.Domain.Uow;

namespace MiniAbp.EntityFramework
{
    public interface IDbContextProvider<out TDbContext> : IDbContextProvider where TDbContext : DbContext
    {
        TDbContext GetDbContext();
    }
}
