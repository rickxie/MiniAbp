using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Domain.Uow
{
    /// <summary>
    /// Used to get/set current <see cref="IUnitOfWork"/>. 
    /// </summary>
    public interface ICurrentUnitOfWorkProvider
    {
        /// <summary>
        /// Gets/sets current <see cref="IUnitOfWork"/>.
        /// </summary>
        IUnitOfWork Current { get; set; }
    }
}
