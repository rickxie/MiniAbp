using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace MiniAbp.Web.Route
{
    public class UrlRouting
    {
        public static UrlRouting Instance = new UrlRouting();
        public void HandleApiService(HttpApplication application, Action<HttpResponse, string> handler)
        {
            var request = application.Request;
            //不包含API/(*) SERVICE的请求直接返回/api/workflowservice/GetMethod
            var rawUrl = request.RawUrl.ToUpper();
            var requestType = GetRequestType(rawUrl, out rawUrl);
            //若为正常请求则忽略
            if (requestType == RequestType.Normal)
            {
                return;
            }
            object param = requestType == RequestType.ServiceFile
                ? GetFileParam(request)
                : GetStringParam(request);

            var routeRx = "([^/]+)/([^/]+)";
            //获取匹配路由
            Regex rx = new Regex(routeRx);
            var result = rx.Match(rawUrl);
            if (result.Groups.Count != 3)
            {
                return;
            }
            var service = result.Groups[1].Value;
            var method = result.Groups[2].Value;
            var outputJson = YRequestHandler.ApiService(service, method, param);
            handler(application.Response, outputJson);
        }

        private static object GetFileParam(HttpRequest request)
        {
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            for (int i = 0; i < request.Files.Count; i++)
            {
                files.Add(request.Files[i]);
            }
            return files;
        }

        private static string GetStringParam(HttpRequest request)
        {
            var str = request.InputStream;
            // Find number of bytes in stream.
            var strLen = Convert.ToInt32(str.Length);
            // Create a byte array.
            byte[] strArr = new byte[strLen];
            // Read stream into byte array.
            str.Read(strArr, 0, strLen);
            var param = request.ContentEncoding.GetString(strArr);
            return param;
        }

        private RequestType GetRequestType(string rawUrl, out string raw)
        {
            if (rawUrl.StartsWith("/API/"))
            {
                raw = rawUrl.Remove(0, 5);
                return RequestType.ServiceData;
            }
            if (rawUrl.StartsWith("/FILEAPI/"))
            {
                raw = rawUrl.Remove(0, 9);
                return RequestType.ServiceFile;
            }
            raw = rawUrl;
            return RequestType.Normal;
        }
    }
}
