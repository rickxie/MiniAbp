//using System.Linq;
//using System.Security.Claims;
//using System.Threading;
//using Microsoft.AspNet.Identity;
//using MiniAbp.Runtime;

//namespace MiniAbp.Web.Runtime
//{
//    public class YSession : ISession
//    {
//        private static readonly YSession Instance = new YSession();

//        private YSession()
//        {
//        }

//        public static YSession GetInstance()
//        {
//            return Instance;
//        }

//        public string UserId
//        {
//            get
//            {
//                var userId = Thread.CurrentPrincipal.Identity.GetUserId();
//                if (string.IsNullOrEmpty(userId))
//                {
//                    return null;
//                }

//                return userId;
//            }
//        }
//        public string LanguageCulture => GetClaims(YConst.LanguageCultrue);

//        //public string Name => GetClaims(ClaimTypes.Name);
       
//        private string GetClaims(string type)
//        {
//            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
//            var claim = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == type);
//            return claim?.Value;
//        }
         
//    }
//}
