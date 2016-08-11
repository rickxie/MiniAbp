using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Identity
{
    public class Const
    {
        #region EmailTmp 邮件模板

        /// <summary>
        /// 邀请模板
        /// </summary>
        public const string InviteEmailTmp = "InviteEmailTmp";
        /// <summary>
        /// 激活模板
        /// </summary>
        public const string ActivateEmailTmp = "ActivateEmailTmp";
        /// <summary>
        /// 验证码模板
        /// </summary>
        public const string ValidationCodeTmp = "ValidationCodeTmp";
        /// <summary>
        /// 未注册用户
        /// </summary>
        public const string UnregisteredUser = "UnregisteredUser";
        /// <summary>
        /// 已注册用户
        /// </summary>
        public const string RegisteredUser = "RegisteredUser";

        #endregion
    }
}
