namespace MiniAbp.Domain.Uow
{
    public interface IDbContextProvider
    {
        IDbContext DbContext { get;  }
    }
}
