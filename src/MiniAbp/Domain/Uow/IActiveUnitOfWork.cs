using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniAbp.Domain.Uow
{
    public interface IActiveUnitOfWork
    {
        /// <summary>
        /// This event is raised when this UOW is successfully completed.
        /// </summary>
        event EventHandler Completed;
        /// <summary>
        /// This event is raised when this UOW is disposed.
        /// </summary>
        event EventHandler Disposed;

        event EventHandler<UnitOfWorkFailedEventArgs> Failed;
        /// <summary>
        /// Is this UOW disposed?
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Saves all changes until now in this unit of work.
        /// This method may be called to apply changes whenever needed.
        /// Note that if this unit of work is transactional, saved changes are also rolled back if transaction is rolled back.
        /// No explicit call is needed to SaveChanges generally, 
        /// since all changes saved at end of a unit of work automatically.
        /// </summary>
        void SaveChanges();

    }
}