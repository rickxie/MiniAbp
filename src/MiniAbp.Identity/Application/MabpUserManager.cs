using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.Claims;
using MiniAbp.Authorization;
using MiniAbp.Domain;
using MiniAbp.Extension;
using MiniAbp.Identity.Model;
using MiniAbp.Identity.Model.LoginModel;
using MiniAbp.Identity.Model.Table;
using MiniAbp.Runtime;

namespace MiniAbp.Identity.Application
{
    public abstract class MabpUserManager<TUser> : ISingletonDependency where TUser: User
    {
        private readonly MabpUserManagerRp<TUser> _userManagerRp;

        protected MabpUserManager(MabpUserManagerRp<TUser> userManagerRp)
        {
            _userManagerRp = userManagerRp;
        }

        #region 登录

        /// <summary>
        /// 邮箱激活链接 自动登录
        /// </summary>
        /// <param name="validationcode"></param>
        /// <param name="param"></param>
        public LoginViewModel AutoLogin(string validationcode, string param)
        {
            var loginModel = new LoginViewModel();
            //解密
            string email, password, emailCode, errorStr;
            _userManagerRp.DecryptUser(validationcode, param, out email, out password, out emailCode);

            //GetUser
            var existsUser = _userManagerRp.GetUserByUsnAndPwd(email, Encryptor.PasswordEncrypt(password));

            //验证合法
            if (!_userManagerRp.ValidateLoginUser(existsUser, emailCode, out errorStr))
            {
                loginModel.Success = false;
                loginModel.Error = errorStr;
            }
            //合法数据
            _userManagerRp.ActiveEmailUser(existsUser);

            loginModel.UsernameOrEmailAddress = email;
            loginModel.Password = password;
            loginModel.Success = true;
            return loginModel;
        }

        public LoginViewModel Login(LoginViewModel loginModel, out TUser userModel)
        {
            var result = new LoginViewModel();
            var usn = loginModel.UsernameOrEmailAddress;
            var pwd = Encryptor.PasswordEncrypt(loginModel.Password);

            var user = _userManagerRp.GetUserByUsnAndPwd(usn, pwd);
            if (user == null || !user.IsActive)
            {
                userModel = null;
                return null;
            }

            var d = new UserIdentity { UserId = user.Id, LanguageCulture = user.Language };
            var identity = GetClaimsPrincipal(d);
            result.Identity = identity;
            userModel = user;
            return result;
        }

        /// <summary>
        /// 域登陆验证
        /// </summary>
        /// <param name="adPath"></param>
        /// <param name="loginModel"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public LoginViewModel LoginWithAd(string adPath, LoginViewModel loginModel, out TUser userModel)
        {
            var usn = loginModel.UsernameOrEmailAddress;
            var pwd = loginModel.Password;
            var user = _userManagerRp.GetUser(usn);
            if (user == null || !user.IsActive)
            {
                userModel = null;
                return null;
            }
            var isValid = IsAuthenticated(adPath, usn, pwd);
            if (isValid)
            {
                var result = new LoginViewModel();
                var d = new UserIdentity {UserId = user.Id, LanguageCulture = user.Language};
                var identity = GetClaimsPrincipal(d);
                result.Identity = identity;
                userModel = user;
                return result;
            }
            userModel = null;
            return null;
        }
        public bool IsAuthenticated(string path, string username, string pwd)
        {
            try
            {
                DirectoryEntry entry = new DirectoryEntry(path, username, pwd);
                // 绑定到本机 AdsObject 以强制身份验证。
                Object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();
                if (null == result)
                {
                    return false;
                }
                // 更新目录中的用户的新路径
                //path = result.Path;
                //_filterAttribute = (String)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                string strMsg = ex.Message;
                //    throw new Exception("对用户进行身份验证时出错。 " + ex.Message);
                return false;
            }
            return true;
        }

        public LoginViewModel UpdateLoginLanguage(string userId, string language)
        {
            var result = new LoginViewModel();
            var d = new UserIdentity { UserId = userId, LanguageCulture = language };
            var identity = GetClaimsPrincipal(d);
            result.Identity = identity;
            return result;
        }

        public static ClaimsIdentity GetClaimsPrincipal(UserIdentity user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(YConst.LanguageCultrue, user.LanguageCulture)
            };
            var identity = new ClaimsIdentity(claims, "ApplicationCookie");
            return identity;
        }
        

        /// <summary>
        /// 被邀请的未注册用户首次登录需填写昵称，修改密码
        /// </summary>
        /// <param name="input"></param>
        public void UpdateActiveUser(TUser input)
        {
            var exist = CheckIsExistUser(input.Id);
           
            exist.Password = Encryptor.PasswordEncrypt(input.Password);
            exist.IsAlreadyActivated = true;
            exist.LangName = input.LangName;
            _userManagerRp.Update(exist);
        }

        #endregion

        #region 注册账号

        /// <summary>
        /// 检查用户并设置验证码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="existUser"></param>
        public TUser CheckUserAndSetConfirmationCode(TUser input, out TUser existUser)
        {
            existUser = _userManagerRp.GetUser(string.IsNullOrEmpty(input.CellPhone)? input.EmailAddress:input.CellPhone);

            if (existUser != null && existUser.IsActive)
                throw new UserFriendlyException("该账号已注册，请登录");

            input = SetConfirmationCode(input);

            return input;
        }

        public TUser SetConfirmationCode(TUser input)
        {
            if (!string.IsNullOrEmpty(input.CellPhone))
                input.PhoneConfirmationCode = _userManagerRp.GetConfirmationCode();
            if (!string.IsNullOrEmpty(input.EmailAddress))
                input.EmailConfirmationCode = _userManagerRp.GetConfirmationCode();

            return input;
        }

        /// <summary>
        /// 插入数据库
        /// </summary>
        /// <param name="existUser"></param>
        /// <param name="newUser"></param>
        public void AddOrUpdateUser(TUser existUser, TUser newUser)
        {
            
            if (existUser == null)
            {
                _userManagerRp.InsertUser(newUser);
            }
            else
            {
                if (!string.IsNullOrEmpty(newUser.CellPhone))
                    existUser.PhoneConfirmationCode = newUser.PhoneConfirmationCode;

                if (!string.IsNullOrEmpty(newUser.EmailAddress))
                {
                    existUser.EmailConfirmationCode = newUser.EmailConfirmationCode;
                    existUser.NickName = newUser.NickName;
                    existUser.Password = Encryptor.PasswordEncrypt(newUser.Password);
                }

                _userManagerRp.UpdateUser(existUser);
            }

            
        }

        /// <summary>
        /// 注册进入，校验验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string RegisterUser(TUser input)
        {
            string result;
            var existUser = _userManagerRp.GetUser(input.CellPhone);
            if (existUser != null)
            {
                if (existUser.PhoneConfirmationCode != input.PhoneConfirmationCode)
                {
                    throw new UserFriendlyException("验证码不正确！");
                }
                if (existUser.LastModificationTime < DateTime.Now.AddMinutes(-30))
                {
                    throw new UserFriendlyException("验证码已过期，请重新发送");
                }
                existUser.NickName = input.NickName;
                existUser.Password = input.Password;
                _userManagerRp.RegisterPhoneUser(existUser);
                result = "注册成功";
            }
            else
            {
                result = "注册失败";
            }

            return result;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool ResetLogin(TUser input)
        {
            var existUser = _userManagerRp.GetUser(string.IsNullOrEmpty(input.CellPhone) ? input.EmailAddress : input.CellPhone);
            if (existUser == null)
                throw new UserFriendlyException("账号信息已过期，请重新登录！");

            if (!string.IsNullOrEmpty(input.CellPhone))
            {
                if (existUser.PhoneConfirmationCode != input.PhoneConfirmationCode)
                    throw new UserFriendlyException("验证码错误！");
                existUser.PhoneConfirmationCode = null;
            }
            else if (!string.IsNullOrEmpty(input.EmailAddress))
            {
                if (existUser.EmailConfirmationCode != input.EmailConfirmationCode)
                    throw new UserFriendlyException("验证码错误！");
                existUser.EmailConfirmationCode = null;
            }
            else
                throw new UserFriendlyException("账号信息已过期，请重新登录");

            existUser.Password = Encryptor.PasswordEncrypt(input.Password);
            _userManagerRp.UpdateUser(existUser);
            return true;
        }

        #endregion

        #region 个人设置

        /// <summary>
        /// 更新个人基本信息
        /// </summary>
        /// <param name="input"></param>
        /// <param name="user"></param>
        public void UpdateUser(TUser user)
        {
            _userManagerRp.Update(user);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="newPassword"></param>
        [MabpAuthorize]
        public void UpdatePassword(string userId, string password, string newPassword)
        {
            //校验密码
            var exist = CheckPassword(password, userId);

            exist.Password = newPassword;
            _userManagerRp.UpdatePwd(exist);
        }

        /// <summary>
        /// 重置 TUser AccessId
        /// </summary>
        /// <param name="input"></param>
        [MabpAuthorize]
        public void ResetAppUserAccessId(TUser input)
        {
            _userManagerRp.UpdateUser(input);
        }


        #endregion

        #region 登录账号解绑与绑定

        /// <summary>
        /// 校验密码
        /// </summary>
        /// <param name="password"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TUser CheckPassword(string password, string userId)
        {
            var exist = _userManagerRp.GetUserByUserId(userId);
            if (exist == null || exist.Password != Encryptor.PasswordEncrypt(password))
            {
                throw new UserFriendlyException("密码错误");
            }
            return exist;
        }

        public TUser CheckAndGetBingdingCode(int step, string usn, string userId)
        {
            //绑定新账号时校验新账号是否已绑定
            if (step == 4)
            {
                var existUser = _userManagerRp.GetUser(usn);
                if (existUser != null)
                    throw new UserFriendlyException("该" + usn + "已绑定！");
            }

            var user = _userManagerRp.GetUserByUserId(userId);
            if (user == null)
                throw new UserFriendlyException("账号信息已过期，请重新登录！");

            user = SetConfirmationCode(user);

            return user;
        }

        /// <summary>
        /// 校验验证码
        /// </summary>
        /// <param name="step"></param>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <param name="code"></param>
        [MabpAuthorize]
        public void CheckValidationCode(int step, string phone, string email, string userId, int type, string code)
        {
            //校验用户是否存在
            var existUser = CheckIsExistUser(userId);
            
            if (!string.IsNullOrEmpty(phone) && type == 1)
            {
                if (existUser.PhoneConfirmationCode != code || code == null)
                    throw new UserFriendlyException("验证码错误！");
                if (existUser.CellPhone != phone)
                {
                    existUser.CellPhone = phone;
                }
                existUser.PhoneConfirmationCode = null;
            }

            else if (!string.IsNullOrEmpty(email) && type == 2)
            {
                if (existUser.EmailConfirmationCode != code || code == null)
                    throw new UserFriendlyException("验证码错误！");
                if (existUser.EmailAddress != email)
                {
                    existUser.EmailAddress = email;
                }
                existUser.EmailConfirmationCode = null;
            }
            else
                throw new UserFriendlyException("账号信息已过期，请重新登录");

            _userManagerRp.UpdateUser(existUser);

        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="type"></param>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [MabpAuthorize]
        public string UnBingding(int type, string phone, string email, string userId, string code)
        {
            //校验用户是否存在
            var existUser = CheckIsExistUser(userId);

            if (!string.IsNullOrEmpty(phone) && type == 1)
            {
                if (existUser.PhoneConfirmationCode != code || code == null)
                    throw new UserFriendlyException("验证码错误！");

                if (string.IsNullOrEmpty(existUser.EmailAddress))
                {
                    existUser.PhoneConfirmationCode = null;
                    _userManagerRp.UpdateUser(existUser);
                    return "邮箱和手机号必须绑定一个！";
                }

                existUser.CellPhone = null;
            }
            else if (!string.IsNullOrEmpty(email) && type == 2)
            {
                if (existUser.EmailConfirmationCode != code || code == null)
                    throw new UserFriendlyException("验证码错误！");

                if (string.IsNullOrEmpty(existUser.CellPhone))
                {
                    existUser.EmailConfirmationCode = null;
                    _userManagerRp.UpdateUser(existUser);
                    return "邮箱和手机号必须绑定一个！";
                }

                existUser.EmailAddress = null;
            }

            _userManagerRp.UpdateUser(existUser);
            return null;
        }

        #endregion

        #region 邀请用户

        public TUser GetUser(string usn)
        {
            return _userManagerRp.GetUser(usn);
        }


        public TUser InsertInitialUser(TUser input)
        {
            return _userManagerRp.InsertInitialUser(input);
        }

        #endregion

        #region base

        /// <summary>
        /// 根据UserId 获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TUserDto GetUserByUserId<TUserDto>(string userId)
        {
            return _userManagerRp.GetUserByUserId<TUserDto>(userId);
        }
        public TUser GetUserByUserId(string userId)
        {
            return _userManagerRp.GetUserByUserId(userId);
        }

        /// <summary>
        /// 根据 CellPhone 或 EmailAddress 和 密码 获取TUser
        /// </summary>
        /// <param name="usn"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public TUser GetUserByUsnAndPwd(string usn, string pwd)
        {
            return _userManagerRp.GetUserByUsnAndPwd(usn, pwd);
        }

        /// <summary>
        /// 校验TUser是否存在
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TUser CheckIsExistUser(string userId)
        {
            var existUser = _userManagerRp.GetUserByUserId(userId);

            if (existUser == null)
                throw new UserFriendlyException("账号信息已过期，请重新登录！");

            return existUser;
        }

        #endregion

        

    }

    public class UserIdentity
    {
        public string UserId { get; set; }
        public string LanguageCulture { get; set; }
    }
}
