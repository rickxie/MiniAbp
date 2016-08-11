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
        private readonly IocManager _iocResolver;

        public AuthorizationInterceptor(IocManager iocResolver)
        {
            _iocResolver = iocResolver;
        }
 
        public void Intercept(IInvocation invocation)
        {
            var authorizeAttrList =
               ReflectionHelper.GetAttributesOfMemberAndDeclaringType<MabpAuthorizeAttribute>(
                   invocation.MethodInvocationTarget
                   );

            if (authorizeAttrList.Count <= 0)
            {
                //No AbpAuthorizeAttribute to be checked
                invocation.Proceed();
                return;
            }

            using (var authorizationAttributeHelper = _iocResolver.ResolveAsDisposable<IAuthorizeAttributeHelper>())
            {
                authorizationAttributeHelper.Object.Authorize(authorizeAttrList);
                invocation.Proceed();
            }
        }
    }
}
