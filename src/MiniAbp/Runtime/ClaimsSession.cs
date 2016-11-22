using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace MiniAbp.Runtime
{
    /// <summary>
    /// Implements <see cref="ISession"/> to get session properties from claims of <see cref="Thread.CurrentPrincipal"/>.
    /// </summary>
    public class ClaimsSession : ISession
    {
        private const int DefaultTenantId = 1;

        public virtual string UserId
        {
            get
            {
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
                if (claimsIdentity == null)
                {
                    return null;
                }

                var userIdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    return null;
                }

            
                return userIdClaim.Value;
            }
        }
        public string LanguageCulture => GetClaims(YConst.LanguageCultrue);

        private string GetClaims(string type)
        {
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var claim = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == type);
            return claim?.Value;
        }

    }
}