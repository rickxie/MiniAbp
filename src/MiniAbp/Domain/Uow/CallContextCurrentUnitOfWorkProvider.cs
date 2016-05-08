using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;

namespace MiniAbp.Domain.Uow
{
    public class CallContextCurrentUnitOfWorkProvider : ICurrentUnitOfWorkProvider, ITransientDependency
    {
        private const string ContextKey = "UnitOfWork.Current";
        private static readonly ConcurrentDictionary<string, IUnitOfWork> UnitOfWorkDictionary
            = new ConcurrentDictionary<string, IUnitOfWork>();

        internal static IUnitOfWork StaticUow
        {
            get
            {
                var unitOfWorkKey = CallContext.LogicalGetData(ContextKey) as string;
                if (unitOfWorkKey == null)
                {
                    return null;
                }
                IUnitOfWork unitOfWork;
                if (!UnitOfWorkDictionary.TryGetValue(unitOfWorkKey, out unitOfWork))
                {
                    CallContext.LogicalSetData(ContextKey, null);
                    return null;
                }
                if (unitOfWork.IsDisposed)
                {
                    CallContext.LogicalSetData(ContextKey, null);
                    UnitOfWorkDictionary.TryRemove(unitOfWorkKey, out unitOfWork);
                    return null;
                }
                return unitOfWork;
            }
            set
            {
                var unitOfWorkKey = CallContext.LogicalGetData(ContextKey) as string;
                if (unitOfWorkKey != null)
                {
                    IUnitOfWork unitOfWork;
                    if (UnitOfWorkDictionary.TryGetValue(unitOfWorkKey, out  unitOfWork))
                    {
                        if (unitOfWork == value)
                        {
                            //Setting same object, no need to set again
                            return;
                        }
                        UnitOfWorkDictionary.TryRemove(unitOfWorkKey, out unitOfWork);
                    }
                    CallContext.LogicalSetData(ContextKey, null);
                }
                if (value == null)
                {
                    //It's already null(because of the logic above), no need to set
                    return;
                }
                unitOfWorkKey = Guid.NewGuid().ToString();
                if (!UnitOfWorkDictionary.TryAdd(unitOfWorkKey, value))
                {
                    //this is almost impossible, but we're checking
                    throw new Exception("Can not set unit of work!");
                }
                CallContext.LogicalSetData(ContextKey, unitOfWorkKey);
            }
        }
        [DoNotWire]
        public IUnitOfWork Current
        {
            get { return StaticUow; } set { StaticUow = value; }
        }
    }
}
