using System;
using MiniAbp.Ado.Repository;
using MiniAbp.Ado.Uow;
using MiniAbp.Domain;
using MiniAbp.Extension;
using MiniAbp.Identity.Model;
using MiniAbp.Identity.Model.Table;

namespace MiniAbp.Identity.Application
{
    public abstract class MabpUserManagerRp<TUser> : AdoRepositoryBase<TUser, string> where TUser:User
    {
        protected MabpUserManagerRp(IDbContextProvider dbContextProvider) : base(dbContextProvider)
        {
        }

        /// <summary>
        /// 根据 CellPhone 或 EmailAddress 获取TUser
        /// </summary>
        /// <param name="usn"></param>
        /// <returns></returns>
        public TUser GetUser(string usn)
        {
            return QueryFirst<TUser>(SqlScript.GetUser,
                new { usn, Lang = Session.LanguageCulture });
        }

        /// <summary>
        /// 根据 CellPhone 或 EmailAddress 和 密码 获取TUser
        /// </summary>
        /// <param name="usn"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public TUser GetUserByUsnAndPwd(string usn, string pwd)
        {
            return QueryFirst<TUser>(SqlScript.GetUserByUsnAndPwd,
                new { usn, pwd, Lang = string.IsNullOrEmpty(Session.LanguageCulture)? "zh-CN" : Session.LanguageCulture });
        }

        /// <summary>
        /// 根据UserId 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TUser GetUserByUserId(string userId)
        {
            return QueryFirst<TUser>(SqlScript.GetUserByUserId, new { userId, Lang = Session.LanguageCulture });
        }
        public TUserDto GetUserByUserId<TUserDto>(string userId)
        {
            return QueryFirst<TUserDto>(SqlScript.GetUserByUserId, new { userId, Lang = Session.LanguageCulture });
        }



        #region 登录

        /// <summary>
        /// 解密账号
        /// </summary>
        /// <param name="validationcode"></param>
        /// <param name="param"></param>
        /// <param name="email"></param>
        /// <param name="pwd"></param>
        /// <param name="code"></param>
        public void DecryptUser(string validationcode, string param, out string email, out string pwd, out string code)
        {
            code = Encryptor.SDecryptString(validationcode);
            var part = Encryptor.SDecryptString(param);

            string[] emailPwd = part.Split(',');
            email = emailPwd[0];
            pwd = emailPwd[1];
        }

        /// <summary>
        /// 验证合法性
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code"></param>
        /// <param name="errorStr"></param>
        /// <returns></returns>
        public bool ValidateLoginUser(TUser user, string code, out string errorStr)
        {
            if (user != null && user.EmailConfirmationCode == code)
            {
                if (user.LastModificationTime < DateTime.Now.AddHours(-24))
                {
                    errorStr = "激活链接已经失效，请重新注册！";
                    return true;
                }
                errorStr = "激活链接已经失效，请重新注册！";
                return true;
            }
            else
            {
                errorStr = "链接不合法！";
                return false;
            }
        }

        /// <summary>
        /// 激活邮箱注册用户
        /// </summary>
        /// <param name="input"></param>
        public void ActiveEmailUser(TUser user)
        {
            user.IsEmailConfirmed = true;
            user.IsActive = true;
            user.EmailConfirmationCode = null;
            user.IsAlreadyActivated = true;
            Update(user);
        }

        #endregion

        #region 注册账号

        /// <summary>
        /// 手机 - 点击发送验证码
        /// </summary>
        /// <param name="user"></param>
        public void InsertUser(TUser user)
        {
            user.RefreshId();
            user.Language = "zh-CN";
            user.LastModificationTime = DateTime.Now;
            user.NickName = user.NickName;
            user.Password = Encryptor.PasswordEncrypt(user.Password);
            Insert(user);
        }

        /// <summary>
        /// 手机 - 点击注册
        /// </summary>
        /// <param name="user"></param>
        public void RegisterPhoneUser(TUser user)
        {
            user.IsActive = true;
            user.Password = Encryptor.PasswordEncrypt(user.Password);
            user.IsPhoneConfirmed = true;
            user.LastModificationTime = DateTime.Now;
            user.PhoneConfirmationCode = null;
            user.IsAlreadyActivated = true;
            Update(user);
        }

        /// <summary>
        /// 邮箱 - 点击注册
        /// </summary>
        /// <param name="user"></param>
        public void InsertEmailUser(TUser user)
        {

            /////InsertUser();
            //user.RefreshId();
            //user.Language = "zh-CN";
            //user.LastModificationTime = DateTime.Now;
            //user.NickName = user.NickName;
            ////user.SetAccessId();
            //user.Password = Encryptor.PasswordEncrypt(user.Password);
            //Insert(user);
        }

        /// <summary>
        /// 跟新TUser
        /// </summary>
        /// <param name="input"></param>
        /// <param name="user"></param>
        public void UpdateUser(TUser user)
        {
            user.LastModificationTime = DateTime.Now;
            Update(user);
        }

        #endregion

        #region 个人设置

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        public void UpdatePwd(TUser input)
        {
            input.Password = Encryptor.PasswordEncrypt(input.Password);
            Update(input);
        }

        #region 登录账号解绑与绑定

        /// <summary>
        /// 设置验证码 Code
        /// </summary>
        /// <returns></returns>
        public string GetConfirmationCode()
        {
            var code = Guid.NewGuid().ToString("N");
            return code.Substring(code.Length - 6);
        }


        #endregion


        #endregion

        /// <summary>
        /// 添加未注册用户，初始化用户数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public TUser InsertInitialUser(TUser input)
        {
            input.RefreshId();
            input.NickName = string.IsNullOrEmpty(input.CellPhone)? input.EmailAddress : input.CellPhone;
            input.IsActive = true;
            input.Password = Encryptor.PasswordEncrypt(input.InitialPassword); //设置默认密码
            input.Language = "zh-CN";
            Insert(input);
            return input;
        }
        
    }
}
