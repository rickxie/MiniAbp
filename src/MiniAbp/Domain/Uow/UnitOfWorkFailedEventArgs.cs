using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Domain.Uow
{
    public class UnitOfWorkFailedEventArgs: EventArgs
    {
        public Exception Exception { get; private set; }
        public UnitOfWorkFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
