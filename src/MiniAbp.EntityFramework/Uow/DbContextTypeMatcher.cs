
using MiniAbp.Domain.Uow;
using MiniAbp.EntityFramework.Common;
using System.Data.Entity;

namespace MiniAbp.EntityFramework
{
    public class DbContextTypeMatcher : DbContextTypeMatcher<DbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider) 
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}