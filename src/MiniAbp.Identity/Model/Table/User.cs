using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity;
using MiniAbp.Domain;

namespace MiniAbp.Identity.Model.Table
{
    [Table("AppUser")]
    public class User: CreationAndDeletionEntity 
    {
        [Key]
        [StringLength(50)]
        public override string Id { get; set; }

        [StringLength(50)]
        public string Account { get; set; }
        [StringLength(128)]
        public string Password { get; set; }
        [StringLength(50)]
        public string LangName { get; set; }
        [StringLength(50)]
        public string Language { get; set; }
        [StringLength(50)]
        public string EmailAddress { get; set; }
        [StringLength(50)]
        public string CellPhone { get; set; }
        public bool IsActive { get; set; } 
        //public bool IsOut { get; set; }
        [StringLength(50)]
        public string InitialPassword { get; set; }
        public bool IsAlreadyActivated { get; set; }
        [StringLength(50)]
        public string AccessId { get; set; }
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
        [StringLength(50)]
        public string PhoneConfirmationCode { get; set; }

        /// <summary>
        /// 邮箱验证码
        /// </summary>
        [StringLength(50)]
        public string EmailConfirmationCode { get; set; }

        public bool IsPhoneConfirmed { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [StringLength(50)]
        public string NickName { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        [StringLength(100)]
        public string Location { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}
