using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.DataAccess;
using System.Transactions;

namespace MiniAbp.Domain.Uow
{
    public interface IUnitOfWorkDefaultOptions
    {
        bool IsTransactional { get; set; }
        TimeSpan? Timeout { get; set; } 
        IsolationLevel? IsolationLevel { get; set; }
        /// <summary>
        /// Scope option.
        /// </summary>
        TransactionScopeOption Scope { get; set; }

        /// <summary>
        /// A list of selectors to determine conventional Unit Of Work classes.
        /// </summary>

        List<Func<Type, bool>> ConventionalUowSelectors { get; }

    }
}
