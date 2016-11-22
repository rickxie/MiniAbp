using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using MiniAbp.Domain;
using MiniAbp.Reflection;
using MiniAbp.Runtime;

namespace MiniAbp.Authorization
{
    internal class AuthorizeHelper : IAuthorizeHelper, ITransientDependency
    {
        public ISession Session { get; set; }

        //public IPermissionChecker PermissionChecker { get; set; }

        public AuthorizeHelper()
        {
            Session = NullSession.GetInstance();
//            PermissionChecker = NullPermissionChecker.Instance;
        }


        public async Task AuthorizeAsync(MethodInfo methodInfo)
        {
            if (AllowAnonymous(methodInfo))
            {
                return;
            }

            ////Authorize
           await CheckPermissions(methodInfo);
        }

        public async Task AuthorizeAsync(IEnumerable<IMabpAuthorizeAttribute> authorizeAttributes)
        {
            if (Session.UserId.IsNullOrEmpty())
            {
                throw new AuthorizationException("No user logged in!", true);
            }
            //await Test();
        }

        public async Task AuthorizeAsync(IMabpAuthorizeAttribute authorizeAttribute)
        {
            if (Session.UserId.IsNullOrEmpty())
            {
                throw new AuthorizationException("No user logged in!", true);
            }
        }

        private static bool AllowAnonymous(MethodInfo methodInfo)
        {
            return ReflectionHelper.GetAttributesOfMemberAndDeclaringType(methodInfo)
                .OfType<MabpAllowAnonymousAttribute>().Any();
        }
         
        private async Task CheckPermissions(MethodInfo methodInfo)
        {
            var authorizeAttributes = ReflectionHelper.GetAttributesOfMemberAndDeclaringType(
                    methodInfo
                ).OfType<IMabpAuthorizeAttribute>().ToArray();

            if (!authorizeAttributes.Any())
            {
                return;
            }

            await AuthorizeAsync(authorizeAttributes);
        }

    }
}
