using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using MiniAbp.Threading;

namespace MiniAbp.Authorization
{
    public static class AuthorizationHelperExtensions
    {
//        public static async Task AuthorizeAsync(this IAuthorizeHelper authorizationHelper, IEnumerable<IMabpAuthorizeAttribute> authorizeAttribute)
//        {
//            await authorizationHelper.AuthorizeAsync(authorizeAttribute);
//        }

        public static void Authorize(this IAuthorizeHelper authorizationHelper, IEnumerable<IMabpAuthorizeAttribute> authorizeAttributes)
        {
            AsyncHelper.RunSync(() => authorizationHelper.AuthorizeAsync(authorizeAttributes));
        }

        public static void Authorize(this IAuthorizeHelper authorizationHelper, IMabpAuthorizeAttribute authorizeAttribute)
        {
            authorizationHelper.Authorize(new[] { authorizeAttribute });
        }

        public static void Authorize(this IAuthorizeHelper authorizationHelper, MethodInfo methodInfo)
        {
            AsyncHelper.RunSync(() => authorizationHelper.AuthorizeAsync(methodInfo));
        }
    }
}