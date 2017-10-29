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


        public void Authorize(MethodInfo methodInfo)
        {
            if (AllowAnonymous(methodInfo))
            {
                return;
            }

            ////Authorize
           CheckPermissions(methodInfo);
        }

        private static bool AllowAnonymous(MethodInfo methodInfo)
        {
            return ReflectionHelper.GetAttributesOfMemberAndDeclaringType(methodInfo)
                .OfType<MabpAllowAnonymousAttribute>().Any();
        }
         
        private void CheckPermissions(MethodInfo methodInfo)
        {
            var authorizeAttributes = ReflectionHelper.GetAttributesOfMemberAndDeclaringType(
                    methodInfo
                ).OfType<IMabpAuthorizeAttribute>().ToArray();

            if (!authorizeAttributes.Any())
            {
                return;
            }
            if (Session.UserId.IsNullOrEmpty())
            {
                throw new AuthorizationException("No user logged in!", true);
            }
        }

    }
}
