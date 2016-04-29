using MiniAbp.Domain.Uow;

namespace MiniAbp.Ado.Uow
{
    public interface IDbContextProvider
    {
        IDbContext DbContext { get;  }
    }
}
