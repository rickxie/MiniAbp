using System;

namespace MiniAbp.Authorization
{
    public class PermissionChecker
    {
        /// <summary>
        /// 判断用户是否有此权限
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool IsGrant(string permission)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 判断当前用户是否已授权
        /// </summary>
        /// <returns></returns>
        public static bool IsAuthenticated()
        {
            throw new NotImplementedException();
        }
    }
}
