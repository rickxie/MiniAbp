using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MiniAbp.Net
{
    /// <summary>
    /// 邮件代理类
    /// </summary>
    public class EmailClient
    {
        public int Port; //发送邮件所用的端口号（htmp协议默认为25）
        public string HostAddress { get; set; }//发件箱的邮件服务器地址（IP形式或字符串形式均可）
        public string SenderDisplayName { get; set; }//发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）
        public string ServerEmail { get; set; } //发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）
        public string ServerPassword { get; set; } //发件箱的密码
        public string ServerUserName { get; set; } //发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）
        public bool EnableSsl { get; set; } //是否对邮件内容进行socket层加密传输
        public bool EnablePwdAuthentication { get; set; } = true; //是否对发件人邮箱进行密码验证
        public int Timeout { get; set; } = 30000;
        public bool UseDefaultCredentials { get; set; } = false;
        public Encoding BodyEncoding { get; set; } = Encoding.UTF8;
        public bool IsHtmlBody { get; set; } = true;
        public AttachmentCollection Attachment { get; set; }

        ///  <summary>
        ///  初始化邮件帮助类
        ///  </summary>
        /// <param name="hostAddress"> 发件箱的邮件服务器地址 </param>
        /// <param name="serverUserName"> 发件人地址</param>
        /// <param name="displaySenderName"> 发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello） </param>
        /// <param name="serverEmail"></param>
        /// <param name="password"> 发件人邮箱密码 </param>
        /// <param name="port"> 发送邮件所用的端口号（htmp协议默认为25） </param>
        /// <param name="sslEnable"> true表示对邮件内容进行socket层加密传输，false表示不加密 </param>
        /// <param name="useDefaultCredentials"></param>
        /// <param name="timeout"></param>
        public EmailClient(string hostAddress, string serverUserName, string serverEmail, string password, int port,
            bool sslEnable, string displaySenderName, bool useDefaultCredentials = false, int timeout = 30000)
        {
            try
            {
                UseDefaultCredentials = useDefaultCredentials;
                Timeout = timeout;
                ServerUserName = serverUserName;
                ServerEmail = serverEmail;
                HostAddress = hostAddress;
                SenderDisplayName = displaySenderName;
                ServerPassword = password;
                Port = port;
                EnableSsl = sslEnable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        /// <summary>
        /// Send Email With subject, body, tolist, or CC or BCC
        /// </summary>
        public void Send(string subject, 
            string mailBody, 
            string[] toUserAddresss, 
            string[] cc = null, 
            string[] bcc = null, 
            string[] attachments = null)
        {
            if (toUserAddresss == null || toUserAddresss.Length == 0)
            {
                return;
            }
            SmtpClient client = new SmtpClient(HostAddress, Port)
            {
                EnableSsl = EnableSsl,
                Credentials = new System.Net.NetworkCredential(ServerUserName, ServerPassword)
            };

            var message = new MailMessage { From = new MailAddress(ServerEmail, SenderDisplayName) };
            foreach (var addr in toUserAddresss)
            {
                message.To.Add(new MailAddress(addr));
            }
            // Attachment.Foreach(r => message.Attachments.Add(r));
            if (attachments != null)
            {
                foreach (var s in attachments)
                {
                    Attachment data = new Attachment(s, MediaTypeNames.Application.Octet);
                    data.Name = s.Substring(s.LastIndexOf("/", StringComparison.Ordinal) + 1);
                    data.NameEncoding = Encoding.UTF8;
                    // Add time stamp information for the file.
                    ContentDisposition disposition = data.ContentDisposition;
                    disposition.CreationDate = File.GetCreationTime(s);
                    disposition.ModificationDate = File.GetLastWriteTime(s);
                    disposition.ReadDate = File.GetLastAccessTime(s);
                    // Add the file attachment to this e-mail message.
                    message.Attachments.Add(data);
                }
            }

//            Set CC list
            if (cc != null)
            {
                foreach (var s in cc)
                {
                    message.CC.Add(s);
                }
            }
            ////Set Bcc List
            if (bcc != null)
            {
                foreach (var s in bcc)
                {
                    message.Bcc.Add(s);
                }
            }
            ServicePointManager.ServerCertificateValidationCallback = delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; };
            message.Subject = subject;
            message.Body = mailBody;
            message.BodyEncoding = BodyEncoding;
            message.IsBodyHtml = IsHtmlBody;
            client.Timeout = Timeout;
            client.Send(message);
        }
    }
}
