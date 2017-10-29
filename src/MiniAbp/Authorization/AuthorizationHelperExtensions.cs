using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using MiniAbp.Threading;

namespace MiniAbp.Authorization
{
    public static class AuthorizationHelperExtensions
    {
        public static void Authorize(this IAuthorizeHelper authorizationHelper, MethodInfo methodInfo)
        {
            authorizationHelper.Authorize(methodInfo);
        }
    }
}