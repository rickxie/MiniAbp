using System.Security.Principal;
using System.Threading;

namespace MiniAbp.Authorization
{
    public class PrincipalHelper
    {
        public static void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
        }
    }
}
