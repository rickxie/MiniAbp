using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Domain.Uow
{
    public interface IUnitOfWork : IActiveUnitOfWork, IUnitOfWorkCompleteHandle
    {
        /// <summary>
        /// Unique id of this UOW.
        /// </summary>
        string Id { get; }
        void Begin(UnitOfWorkOptions options);
    }
}
