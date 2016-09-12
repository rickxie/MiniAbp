using System.Collections.Generic;
using Castle.DynamicProxy;
using MiniAbp.Dependency;
using MiniAbp.Reflection;

namespace MiniAbp.Authorization.Interceptors
{
    /// <summary>
    /// This class is used to intercept methods to make authorization if the method defined <see cref="MabpAuthorizeAttribute"/>.
    /// </summary>
    public class AuthorizationInterceptor : IInterceptor
    {
        private readonly IAuthorizeHelper _authorizeHelper;

        public AuthorizationInterceptor(IAuthorizeHelper authorizeHelper)
        {
            _authorizeHelper = authorizeHelper;
        }

        /// <summary>
        /// method authorization check
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            _authorizeHelper.Authorize(invocation.MethodInvocationTarget);
            invocation.Proceed();
        }
    }
}
