using System;
using System.Threading.Tasks;

namespace MiniAbp.Domain.Uow
{
    public interface IUnitOfWorkCompleteHandle : IDisposable
    {
        void Complete();
        /// <summary>
        /// Completes this unit of work.
        /// It saves all changes and commit transaction if exists.
        /// </summary>
        Task CompleteAsync();
    }
}