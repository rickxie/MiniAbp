using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Authorization
{ 
        public interface IAuthorizeHelper
        {
            Task AuthorizeAsync(IEnumerable<IMabpAuthorizeAttribute> authorizeAttributes);
            Task AuthorizeAsync(IMabpAuthorizeAttribute authorizeAttribute);
            Task AuthorizeAsync(MethodInfo methodInfo);
        }
}
