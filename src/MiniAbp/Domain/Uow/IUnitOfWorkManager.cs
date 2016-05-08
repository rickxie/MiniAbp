using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Domain.Uow
{
    public interface IUnitOfWorkManager
    {
        IActiveUnitOfWork Current { get; }
        IUnitOfWorkCompleteHandle Begin();
        IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options);
    }
}
