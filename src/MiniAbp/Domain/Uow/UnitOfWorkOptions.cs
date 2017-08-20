using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.DataAccess;
using System.Transactions;

namespace MiniAbp.Domain.Uow
{
    public class UnitOfWorkOptions
    {
        public bool? IsTransactional { get; set; }
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// If this UOW is transactional, this option indicated the isolation level of the transaction.
        /// Uses default value if not supplied.
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }
        /// <summary>
        /// Scope option.
        /// </summary>
        public TransactionScopeOption? Scope { get; set; }
        internal void FillDefaultsForNonProvidedOptions(IUnitOfWorkDefaultOptions defaultOptions)
        {
            //TODO: Do not change options object..?

            if (!IsTransactional.HasValue)
            {
                IsTransactional = defaultOptions.IsTransactional;
            }

            if (!Scope.HasValue)
            {
                Scope = defaultOptions.Scope;
            }

            if (!Timeout.HasValue && defaultOptions.Timeout.HasValue)
            {
                Timeout = defaultOptions.Timeout.Value;
            }

            if (!IsolationLevel.HasValue && defaultOptions.IsolationLevel.HasValue)
            {
                IsolationLevel = defaultOptions.IsolationLevel.Value;
            }
        }
    }
}
