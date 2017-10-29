using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace MiniAbp.Net
{
    public static class WebRequestHelper
    {
        #region 发送POST

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sParaTemp"></param>
        /// <param name="headers"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string SendPostRequest(string url, IDictionary<string, string> sParaTemp, IDictionary<object, string> headers = null, CookieContainer cookie = null)
        {
            return SendPostRequest((HttpWebRequest)WebRequest.Create(url), Encoding.UTF8, sParaTemp, headers, cookie);
        }

        /// <summary>
        ///  Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <param name="sParaTemp"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string SendPostRequest(string url, Encoding encoding, IDictionary<string, string> sParaTemp, IDictionary<object, string> headers = null)
        {
            return SendPostRequest((HttpWebRequest)WebRequest.Create(url), encoding, sParaTemp, headers);
        }
        private static string GetParaJoinedString(IDictionary<string, string> sParaTemp)
        {
            var sPara = new StringBuilder();
            if (sParaTemp != null)
            {
                foreach (var val in sParaTemp)
                {
                    sPara.AppendFormat("{0}={1}&", val.Key, val.Value);
                }
                sPara.Remove(sPara.Length - 1, 1);
            }
            return sPara.ToString();
        }
        /// <summary>
        ///  Post请求
        /// </summary>
        /// <param name="myReq"></param>
        /// <param name="encoding"></param>
        /// <param name="sParaTemp"></param>
        /// <param name="headers"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string SendPostRequest(HttpWebRequest myReq, Encoding encoding, IDictionary<string, string> sParaTemp, IDictionary<object, string> headers = null, CookieContainer cookie = null)
        {
            var code = encoding;           
            if (headers != null)
            {
                foreach (var v in headers)
                {
                    if (v.Key is HttpRequestHeader)
                        myReq.Headers[(HttpRequestHeader)v.Key] = v.Value;
                    else
                        myReq.Headers[v.Key.ToString()] = v.Value;
                }
            }
            string strRequestData = GetParaJoinedString(sParaTemp).ToString();
            byte[] bytesRequestData = code.GetBytes(strRequestData);
            try
            {
                myReq.Method = "post";
                myReq.ContentType = "application/x-www-form-urlencoded";
                myReq.ContentLength = bytesRequestData.Length;
                if (cookie != null)
                {
                    myReq.CookieContainer = cookie;
                }
                var requestStream = myReq.GetRequestStream();
                requestStream.Write(bytesRequestData, 0, bytesRequestData.Length);
                requestStream.Close();
                var httpWResp = (HttpWebResponse)myReq.GetResponse();
                var myStream = httpWResp.GetResponseStream();
                if (myStream == null)
                    return null;
                var reader = new StreamReader(myStream, code);
                var result = reader.ReadToEnd();
                myStream.Close();
                return result;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        /// <summary>
        ///  Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SendPostRequest(string url, Encoding encoding, string content)
        {
            return SendPostRequest((HttpWebRequest)WebRequest.Create(url), Encoding.UTF8, content);
        }

        /// <summary>
        ///  Post请求
        /// </summary>
        /// <param name="myReq"></param>
        /// <param name="encoding"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SendPostRequest(HttpWebRequest myReq, Encoding encoding, string content)
        {
            string result = "";//返回结果
            HttpWebResponse httpWResp = null;

            var code = encoding;
            byte[] bytesRequestData = code.GetBytes(content);
            try
            {
                myReq.Method = "post";
                myReq.ContentType = "application/x-www-form-urlencoded";
                myReq.ContentLength = bytesRequestData.Length;
                var requestStream = myReq.GetRequestStream();
                requestStream.Write(bytesRequestData, 0, bytesRequestData.Length);
                requestStream.Close();
                httpWResp = (HttpWebResponse)myReq.GetResponse();
                var myStream = httpWResp.GetResponseStream();
                if (myStream == null)
                    return null;
                var reader = new StreamReader(myStream, code);
                result = reader.ReadToEnd();
                myStream.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //关闭连接和流
                if (httpWResp != null)
                {
                    httpWResp.Close();
                }
                if (myReq != null)
                {
                    myReq.Abort();
                }
            }
            return result;
        }
        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sParaTemp"></param>
        /// <param name="headers"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string SendGetRequest(string url, IDictionary<string, string> sParaTemp, Dictionary<string, string> headers = null, CookieCollection cookies = null )
        {
            var para = GetParaJoinedString(sParaTemp);
            var getUrl = url + "?" + para;
            var response = CreateGetHttpResponse(getUrl, 300, "", cookies, "", headers, "application/x-www-form-urlencoded", true);
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
        /// <summary>
        /// Get Request Advance version
        /// </summary>
        /// <param name="url"></param>
        /// <param name="responseEncoding"></param>
        /// <param name="headers"></param>
        /// <param name="cookies"></param>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="referer"></param>
        /// <param name="contentType"></param>
        /// <param name="keepAlive"></param>
        /// <param name="accept"></param>
        /// <returns></returns>
        public static string SendGetRequest(string url, Encoding responseEncoding, Dictionary<string, string> headers = null, CookieCollection cookies = null, int? timeout = 300, string userAgent = ""
     , string referer = "",  string contentType = "application/x-www-form-urlencoded", bool? keepAlive = true, string accept = "*/*")
        {
            var response = CreateGetHttpResponse(url, timeout, userAgent, cookies, referer, headers, contentType, keepAlive);
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), responseEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <param name="referer"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="keepAlive"></param>
        /// <param name="accept"></param>
        /// <returns></returns>  
        private static HttpWebResponse CreateGetHttpResponse(string url, int? timeout = 300, string userAgent = "", CookieCollection cookies = null
            , string referer = "", Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded", bool? keepAlive = true, string accept = "*/*")
        {

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            HttpWebRequest request;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback((a, b, c, d) => true);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            //if (Proxy != null)
            //{
            //    request.Proxy = Proxy;
            //}

            request.Method = "GET";
            request.Headers["Pragma"] = "no-cache";
            request.Accept = accept;
            request.Headers["Accept-Language"] = "en-US,en;q=0.5";

            request.ContentType = contentType;

            //request.UserAgent = DefaultUserAgent;
            request.Referer = referer;
            if (keepAlive.HasValue)
            {
                request.KeepAlive = keepAlive.Value;
            }
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }


            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value * 1000;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //else
            //{
            //    request.CookieContainer = new CookieContainer();
            //    request.CookieContainer.Add(Cookies);
            //}
            var v = request.GetResponse() as HttpWebResponse;

            return v;
        }
        #endregion


        #region 证书服务
        /// <summary>
        /// 创建带证书设置的httpwebrequest
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="issuerName">颁发者名称</param>
        /// <param name="friendlyName">友好名称</param>
        /// <param name="validOnly">仅限有效证书</param>
        /// <returns></returns>
        public static WebRequest CreateWebRequestWithCertificate(string url, string issuerName, string friendlyName,
            bool validOnly)
        {

            HttpWebRequest request = null;
            var cert = CreateX509Certificate(null, null, issuerName, friendlyName, validOnly);
            if (cert != null)
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                //request.ProtocolVersion = HttpVersion.Version10;
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback((a, b, c, d) => true);
                request.ClientCertificates.Add(cert);
            }
            return request;
        }

        /// <summary>
        /// 创建带证书设置的httpwebrequest
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="certFileName">证书名全路径</param>
        /// <param name="certPassword">证书密码</param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static WebRequest CreateWebRequestWithCertificate(string url, string certFileName, string certPassword, string content)
        {
            HttpWebRequest request = null;
            var cert = CreateX509Certificate(certFileName, certPassword, null, null);
            if (cert != null)
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                //request.ProtocolVersion = HttpVersion.Version10;
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback((a, b, c, d) => true);
                request.ClientCertificates.Add(cert);
            }

            return request;
        }
        /// <summary>
        /// 获取指定证书
        /// </summary>
        /// <param name="issuerName">颁发者名称</param>
        /// <param name="friendlyName">友好名称</param>
        /// <param name="validOnly">仅限有效证书</param>
        /// <returns></returns>
        public static X509Certificate CreateX509Certificate(string issuerName, string friendlyName, bool validOnly)
        {
            return CreateX509Certificate(null, null, issuerName, friendlyName, validOnly);
        }
        /// <summary>
        /// 获取指定证书
        /// </summary>
        /// <param name="certFileName">证书名全路径</param>
        /// <param name="certPassword">证书密码</param>
        /// <returns></returns>
        public static X509Certificate CreateX509Certificate(string certFileName, string certPassword)
        {
            return CreateX509Certificate(certFileName, certPassword, null, null);
        }

        /// <summary>
        /// 获取指定证书
        /// </summary>
        /// <param name="certFileName">证书名全路径</param>
        /// <param name="certPassword">证书密码</param>
        /// <param name="issuerName">颁发者名称</param>
        /// <param name="friendlyName">友好名称</param>
        /// <param name="validOnly">仅限有效证书</param>
        /// <returns></returns>
        private static X509Certificate CreateX509Certificate(string certFileName, string certPassword, string issuerName,
            string friendlyName, bool validOnly = true)
        {
            X509Certificate cert = null;
            if (!string.IsNullOrEmpty(friendlyName))
            {
                var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                try
                {
                    certStore.Open(OpenFlags.ReadOnly);
                    //证书需导入 本地计算机-》个人目录 并同时导入 受信任的根证书颁发机构。 设置权限
                    foreach (
                        var cer in
                            (string.IsNullOrEmpty(issuerName)
                                ? certStore.Certificates
                                : certStore.Certificates.Find(X509FindType.FindByIssuerName, issuerName, validOnly))
                        )
                    {
                        //throw new Exception(cer.FriendlyName);
                        if (cer.FriendlyName != friendlyName) continue;
                        cert = cer;
                        break;
                    }

                }
                finally
                {
                    certStore.Close();
                }
            }

            //没有获取到证书，则从文件创建
            if (cert == null && !string.IsNullOrEmpty(certFileName))
            {

                try
                {
                    var password = GetSecureString(certPassword);
                    cert = password == null
                        ? new X509Certificate(certFileName)
                        : new X509Certificate(certFileName, password);
                }
                catch
                {
                    // ignored
                }
            }
            return cert;
        }

        /// <summary>
        /// 转换成SecureString
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private static SecureString GetSecureString(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return null;
            }
            // Instantiate the secure string.
            var pasSecureString = new SecureString();
            // Use the AppendChar method to add each char value to the secure string.
            foreach (var ch in password)
                pasSecureString.AppendChar(ch);
            return pasSecureString;
        }
        #endregion
    }
}
