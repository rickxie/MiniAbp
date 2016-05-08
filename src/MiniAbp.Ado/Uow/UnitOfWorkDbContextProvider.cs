using System.Data;
using MiniAbp.Domain.Uow;

namespace MiniAbp.Ado.Uow
{
    public class UnitOfWorkDbContextProvider: IDbContextProvider
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        public UnitOfWorkDbContextProvider(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }
        public IDbContext DbContext => _currentUnitOfWorkProvider.Current.GetDbContext();
    }
}
