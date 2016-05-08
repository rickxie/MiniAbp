using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Domain.Uow
{/// <summary>
 /// A helper class to simplify unit of work process.
 /// </summary>
    internal static class UnitOfWorkHelper
    {
        /// <summary>
        /// Returns true if UOW must be used for given type as convention.
        /// </summary>
        /// <param name="type">Type to check</param>
        public static bool IsConventionalUowClass(Type type)
        {
            return typeof(IRepository).IsAssignableFrom(type) || typeof(IApplicationService).IsAssignableFrom(type);
        }
         
    }
}
