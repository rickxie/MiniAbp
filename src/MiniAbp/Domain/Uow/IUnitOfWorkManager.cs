using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MiniAbp.Domain.Uow
{
    public interface IUnitOfWorkManager
    {
        IActiveUnitOfWork Current { get; }
        IUnitOfWorkCompleteHandle Begin();
        IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options);

        /// <summary>
        /// Begins a new unit of work.
        /// </summary>
        /// <returns>A handle to be able to complete the unit of work</returns>
        IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope);
    }
}
