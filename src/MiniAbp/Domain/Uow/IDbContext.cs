using System.Data;

namespace MiniAbp.Domain.Uow
{
    public interface IDbContext
    {
        IDbConnection DbConnection { get; set; }
        IDbTransaction DbTransaction { get; set; } 
    }
}
