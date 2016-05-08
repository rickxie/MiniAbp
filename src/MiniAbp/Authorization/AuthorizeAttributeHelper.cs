using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using MiniAbp.Domain;
using MiniAbp.Runtime;

namespace MiniAbp.Authorization
{
    internal class AuthorizeAttributeHelper : IAuthorizeAttributeHelper, ITransientDependency
    {
        public YSession Session => YSession.GetInstance();

        //public IPermissionChecker PermissionChecker { get; set; }

        public AuthorizeAttributeHelper()
        {
//            AbpSession = NullAbpSession.Instance;
//            PermissionChecker = NullPermissionChecker.Instance;
        }

        

        public Task AuthorizeAsync(IMabpAuthorizeAttribute authorizeAttribute)
        {
            throw new NotImplementedException();
        }

        public void Authorize(IEnumerable<IMabpAuthorizeAttribute> authorizeAttributes)
        {
            if (Session.UserId.IsNullOrEmpty())
            {
                throw new AuthorizationException("No user logged in!", true);
            }


            //            foreach (var authorizeAttribute in authorizeAttributes)
            //            {
            //                await PermissionChecker.AuthorizeAsync(authorizeAttribute.RequireAllPermissions, authorizeAttribute.Permissions);
            //            }
        }

        public void Authorize(IMabpAuthorizeAttribute authorizeAttribute)
        {
            throw new NotImplementedException();
        }
    }
}
