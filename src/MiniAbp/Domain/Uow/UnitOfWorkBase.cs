using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Extension;

namespace MiniAbp.Domain.Uow
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {

        public string Id { get; }
        /// <summary>
        /// Gets the connection string resolver.
        /// </summary>
        protected IConnectionStringResolver ConnectionStringResolver { get; }

        /// <summary>
        /// Gets default UOW options.
        /// </summary>
        protected IUnitOfWorkDefaultOptions DefaultOptions { get; }
        /// <summary>
        /// Constructor.
        /// </summary>
        protected UnitOfWorkBase(
            IConnectionStringResolver connectionStringResolver,
            IUnitOfWorkDefaultOptions defaultOptions)
        {
            DefaultOptions = defaultOptions;
            ConnectionStringResolver = connectionStringResolver;

            Id = Guid.NewGuid().ToString("N");
        }

        public event EventHandler Completed;
        public event EventHandler Disposed;
        public event EventHandler<UnitOfWorkFailedEventArgs> Failed;
        private bool _isBeginCalledBefore;
        private bool _isCompleteCalledBefore;
        private bool _succeed;
        private Exception _exception;
        public UnitOfWorkOptions Options { get; private set; }

        /// <summary>
        /// Should be implemented by derived classes to start UOW.
        /// </summary>
        protected abstract void BeginUow();

        /// <summary>
        /// Should be implemented by derived classes to complete UOW.
        /// </summary>
        protected abstract void CompleteUow();

        ///// <summary>
        ///// Should be implemented by derived classes to complete UOW.
        ///// </summary>
        //protected abstract Task CompleteUowAsync();

        /// <summary>
        /// Should be implemented by derived classes to dispose UOW.
        /// </summary>
        protected abstract void DisposeUow();
        public bool IsDisposed { get; private set; }
        public virtual void SaveChanges()
        {
        }

        public void Complete()
        {
            PreventMultipleComplete();
            try
            {
                CompleteUow();
                _succeed = true;
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }
        public void Begin(UnitOfWorkOptions options)
        {
            Check.NotNull(options, nameof(options));
            PreventMultipleBegin();
            Options = options; 
            BeginUow();
        }
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (!_succeed)
            {
                OnFailed(_exception);
            }

            DisposeUow();
            OnDisposed();
        }

        /// <summary>
        /// Called to trigger <see cref="Failed"/> event.
        /// </summary>
        /// <param name="exception">Exception that cause failure</param>
        protected virtual void OnFailed(Exception exception)
        {
            Failed.InvokeSafely(this, new UnitOfWorkFailedEventArgs(exception));
        }
        /// <summary>
        /// Called to trigger <see cref="Completed"/> event.
        /// </summary>
        protected virtual void OnCompleted()
        {
            Completed.InvokeSafely(this);
        }
        protected virtual void OnDisposed()
        {
            Disposed.InvokeSafely(this);
        }


        private void PreventMultipleBegin()
        {
            if (_isBeginCalledBefore)
            {
                throw new Exception("This unit of work has started before. Can not call Start method more than once.");
            }

            _isBeginCalledBefore = true;
        }
        private void PreventMultipleComplete()
        {
            if (_isCompleteCalledBefore)
            {
                throw new Exception("Complete is called before!");
            }

            _isCompleteCalledBefore = true;
        }
        protected virtual string ResolveConnectionString(ConnectionStringResolveArgs args)
        {
            return ConnectionStringResolver.GetNameOrConnectionString(args);
        }

        public async Task CompleteAsync()
        {
            PreventMultipleComplete();
            try
            {
                await CompleteUowAsync();
                _succeed = true;
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }
        protected abstract Task CompleteUowAsync();

        public abstract Task SaveChangesAsync();

    }
}
