using System;

namespace MiniAbp.Identity.Application
{
    public class UserDto
    {
        public string Id { get; set; }

        public string Account { get; set; }
        public string Password { get; set; }
        public string LangName { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public string EmailAddress { get; set; }
        public string CellPhone { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string InitialPassword { get; set; }
        public bool IsAlreadyActivated { get; set; }

        /// <summary>
        /// 登录连续错误次数（5次）
        /// </summary>
        public int ErrorLoginTimes { get; set; }

        /// <summary>
        /// 最后一次登录错误的时间
        /// </summary>
        public DateTime? LastErrorLoginTime { get; set; }

        /// <summary>
        /// 手机验证码
        /// </summary>
        public string PhoneConfirmationCode { get; set; }

        /// <summary>
        /// 邮箱验证码
        /// </summary>
        public string EmailConfirmationCode { get; set; }

        public bool IsPhoneConfirmed { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public string NickName { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        public string Location { get; set; }

        public DateTime? LastModificationTime { get; set; }


        #region UserModel
        public string EnterpriseId { get; set; }

        public string UserIdentity { get; set; }


        public string NewPassword { get; set; }

        public string AccessId { get; set; }

        #endregion

        public string UserId { get; set; }

        public string Type { get; set; }

    }
}
