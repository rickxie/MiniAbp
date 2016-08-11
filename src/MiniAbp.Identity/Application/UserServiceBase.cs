using System;
using System.Collections.Generic;
using System.Security.Claims;
using Message.Proxy;
using MiniAbp.Domain;
using MiniAbp.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MiniAbp.Identity.Application
{
    public class UserServiceBase: ISingletonDependency
    {
        protected static JObject JObject(string jsonStr)
        {
            return ((JObject)JsonConvert.DeserializeObject(jsonStr));
        }

        /// <summary>
        /// 加密 邮箱，密码
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public string EncryptEmailAndPwd(string email, string pwd)
        {
            string[] part = { email, pwd };
            var emailPwd = string.Join(",", part);
            var encryption = Encryptor.SEncryptString(emailPwd);
            return encryption;
        }

 



        /// <summary>
        /// 解密账号
        /// </summary>
        /// <param name="validationcode"></param>
        /// <param name="param"></param>
        /// <param name="email"></param>
        /// <param name="pwd"></param>
        /// <param name="code"></param>
        public void EncryEmailAndPwdAndCode(string validationcode, string param, out string email, out string pwd, out string code)
        {
            code = Encryptor.SDecryptString(validationcode);
            var part = Encryptor.SDecryptString(param);

            string[] emailPwd = part.Split(',');
            email = emailPwd[0];
            pwd = emailPwd[1];
        }

        /// <summary>
        /// ActivateEmailTmp 账号激活邮件模板
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GetActivateEmailTmp(string email, string code, string password)
        {
            var emailConfirmationCode = Encryptor.SEncryptString(code);
            var encryption = EncryptEmailAndPwd(email, password);

            var ttemp =
            string.Format(
                "<a href=\"https://designer.shalu.com/account/autologin?validationcode={0}&param={1}\">https://designer.shalu.com/account/autologin?validationcode={0}&param={1}</a>",
                emailConfirmationCode, encryption);

            var paramsss = JsonConvert.SerializeObject(new List<string>()
                        {
                            email,
                            ttemp
                        });

            return paramsss;
        }

        /// <summary>
        /// ValidationCodeTmp 邮箱验证码模板
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetValidationCodeTmp(string email, string code)
        {
            var paramsss = JsonConvert.SerializeObject(new List<string>()
                        {
                            email,
                            code
                        });

            return paramsss;
        }

        /// <summary>
        /// InviteEmailTmp 邮箱邀请模板
        /// </summary>
        /// <param name="type"></param>
        /// <param name="email"></param>
        /// <param name="inviter"></param>
        /// <param name="enterpriseName"></param>
        /// <param name="initialPwd"></param>
        /// <returns></returns>
        public string GetInviteEmailTmp(string type, string email, string inviter, string enterpriseName, string initialPwd)
        {
            var paramsss = string.Empty;
            if (type == Const.UnregisteredUser)
            {
                paramsss = JsonConvert.SerializeObject(new List<string>()
                        {
                            email + ", 您好:" ,
                            "您被" + inviter + "邀请加入" + enterpriseName + ", " + "账号为：" + email + " , 初始密码为：",
                            initialPwd,
                            "请登录https://designer.shalu.com , 修改密码。感谢您的支持！"
                        });
            }
            if (type == Const.RegisteredUser)
            {
                paramsss = JsonConvert.SerializeObject(new List<string>()
                        {
                            email + ", 您好:" ,
                            "您被" + inviter + "邀请加入" + enterpriseName + "。" ,
                            "",
                            "请登录https://designer.shalu.com , 修改密码。感谢您的支持！"
                        });
            }

            return paramsss;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <returns></returns>
        public bool SendEmail(string email, string title, string temp, string param)
        {
            OAuthClient oAuthClient = new OAuthClient("qwe", "asd", "http://msg.shalu.com/",
                 OAuthType.ClientIdAndSecrect);
            var result = oAuthClient.Post("/api/services/app/message/SendEmail",
                string.Format("{{'to': '{0}', 'subject':'{1}', 'templateName':'{2}', 'params':{3}}}", email, title, temp,
                   param));
            var succ = JObject(result);
            if (succ["result"].ToString().Equals("100"))
            {
                return true;
            }
            throw new UserFriendlyException("请重新发送验证码或联系管理员");
        }

        /// <summary>
        /// InvitePhoneTmp 短信邀请模板
        /// </summary>
        /// <param name="type"></param>
        /// <param name="phone"></param>
        /// <param name="inviter"></param>
        /// <param name="enterpriseName"></param>
        /// <param name="initialPwd"></param>
        /// <returns></returns>
        public string GetInvitePhoneTmp(string type, string phone, string inviter, string enterpriseName, string initialPwd)
        {
            var paramsss = string.Empty;
            if (type == Const.UnregisteredUser)
            {
                paramsss = phone + "您好, 您被"
                    + inviter
                    + "邀请加入" + enterpriseName + ", "
                    + "账号为：" + phone + ", 初始密码为：" + initialPwd
                    + "。请登录https://designer.shalu.com 修改密码";
            }
            if (type == Const.RegisteredUser)
            {
                paramsss = phone + "您好, 您被" + inviter + "邀请加入"
                           + enterpriseName + "。请使用本手机号登录https://designer.shalu.com ";
            }

            return paramsss;
        }

        /// <summary>
        /// 发送手机短信
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool SendPhone(string name, string param)
        {
            //发送验证码  
            OAuthClient oAuthClient = new OAuthClient("qwe", "asd", "http://msg.shalu.com/",
            OAuthType.ClientIdAndSecrect);
            var result = oAuthClient.Post("/api/services/app/message/SendValidationCode",
                JsonConvert.SerializeObject(
                            new
                            {
                                to = name,
                                content = param
                            }));

            var succ = JObject(result);
            if (succ["result"].ToString().Equals("100"))
            {
                return true;
            }
            throw new UserFriendlyException("请重新发送验证码或联系管理员");
        }


    }
}
