using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Web.Razor
{
    public interface IProxyCreator
    {
        /// <summary>
        /// Create proxy javascript
        /// </summary>
        void CreateProxy(List<Type> serviceType, List<Type> serviceInterface);
    }
}
