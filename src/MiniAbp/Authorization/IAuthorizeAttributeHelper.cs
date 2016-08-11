using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Authorization
{ 
        public interface IAuthorizeAttributeHelper
        {
            void Authorize(IEnumerable<IMabpAuthorizeAttribute> authorizeAttributes);

            void Authorize(IMabpAuthorizeAttribute authorizeAttribute);
        } 
}
