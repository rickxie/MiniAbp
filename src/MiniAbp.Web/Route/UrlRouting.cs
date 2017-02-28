using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using MiniAbp.Dependency;
using MiniAbp.Domain.Entitys;
using MiniAbp.Web.Auditing;
using MiniAbp.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using FileInfo = MiniAbp.Domain.Entitys.FileInfo;
using FilePathResult = System.Web.Mvc.FilePathResult;

namespace MiniAbp.Web.Route
{
    public class UrlRouting
    {
        public static UrlRouting Instance = new UrlRouting(); 

        static UrlRouting()
        {
        }
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="application"></param>
        public void HandleApiService(HttpApplication application)
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

            const string routeRx = "([^/]+)/([^/]+)";
            //获取匹配路由
            Regex rx = new Regex(routeRx);
            var result = rx.Match(rawUrl);
            if (result.Groups.Count != 3)
            {
                return;
            }
            var service = result.Groups[1].Value;
            var method = result.Groups[2].Value;

            object param = requestType == RequestType.ServiceFile
                ? GetFileParam(request)
                : GetStringParam(request);
            
            HandleJsonRequest(service, method, param, application.Response);
        }

        /// <summary>
        /// 获取普通文件请求
        /// </summary>
        /// <param name="service"></param>
        /// <param name="method"></param>
        /// <param name="param"></param>
        /// <param name="response"></param>
        private static void HandleJsonRequest(string service, string method, object param, HttpResponse response)
        {
            var auditing = IocManager.Instance.Resolve<AuditingManager>();
            auditing.Start(service, method, param.ToString());
            var outputObj = YRequestHandler.ApiService(service, method, param);
            var resonseString = string.Empty;
            if (outputObj.Result is FileOutput)
            {}
            else
            {
                resonseString = JsonConvert.SerializeObject(outputObj, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                auditing._auditInfo.ResponseJson = resonseString;
            }
            auditing.Exception(outputObj.Exception);
            auditing.Stop(resonseString);

            if (outputObj.Result is FileOutput)
            {
               ResponseFile(response, outputObj.Result);
            }
            else
            {
               ResponseJson(response, resonseString);
            }
        }

 

        /// <summary>
        /// 返回文件类型
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="response"></param>
        /// <param name="fileObject"></param>
        private static void ResponseFile(HttpResponse response, object fileObject)
        {
            if (fileObject is FilePathOutput)
            {
                var fileResult = fileObject as FilePathOutput;
                 System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileResult.Path);
                response.Clear();
               response.ClearContent();
               response.ClearHeaders();
                response.AddHeader("Content-Disposition",
                    "attachment;filename=" + HttpUtility.UrlPathEncode(fileResult.DownloadName));
               response.AddHeader("Content-Length", fileInfo.Length.ToString());
               response.AddHeader("Content-Transfer-Encoding", "binary");
               response.ContentType = fileResult.ContentType;
               response.ContentEncoding = System.Text.Encoding.UTF8;
               response.WriteFile(fileInfo.FullName);
               response.Flush();
               response.End();
            }
            else if (fileObject is FileStreamOutput)
            {
                var fileResult = fileObject as FileStreamOutput;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.ContentType = fileResult.ContentType;
                
                //通知浏览器下载文件而不是打开
                response.AddHeader("Content-Disposition",
                    "attachment;filename=" + HttpUtility.UrlPathEncode(fileResult.DownloadName));
                response.ContentEncoding = Encoding.UTF8;
                response.Charset = "UTF-8";
                response.WriteFile(fileResult);
                response.Flush();
                response.End();
            } 
        }

        /// <summary>
        /// 返回Json数据
        /// </summary>
        /// <param name="response"></param>
        /// <param name="jsonString"></param> 
        private static void ResponseJson(HttpResponse response, string jsonString)
        {
            response.ContentType = "application/json; charset=utf-8";
            response.Write(jsonString);
            response.End();
        }

        /// <summary>
        /// 获取字符串请求
        /// </summary>
        /// <returns></returns>
        private static string HandleFileRequest()
        {
            throw new Exception("");
        }

        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static object GetFileParam(HttpRequest request)
        {
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            if (request.Files.Count == 0)
            {
                return GetStringParam(request);
            }
            for (int i = 0; i < request.Files.Count; i++)
            {
                files.Add(request.Files[i]);
            }
            var fileInput = GetFileInput(files);
            fileInput.Form = request.Form;
            return fileInput;
        }
        private static FileInput GetFileInput(List<HttpPostedFile> f)
        {
            var fileInput = new FileInput { Files = new List<FileInfo>() };
            if (f != null)
                foreach (var httpPostedFile in f)
                {
                    var file = new FileInfo();
                    file.ContentLength = httpPostedFile.ContentLength;
                    file.ContentType = httpPostedFile.ContentType;
                    file.FileName = httpPostedFile.FileName;
                    file.ExtensionName = Path.GetExtension(file.FileName);
                    var bytes = new byte[file.ContentLength];
                    httpPostedFile.InputStream.Read(bytes, 0, file.ContentLength);
                    file.FileBytes = bytes;
                    fileInput.Files.Add(file);
                }
            return fileInput;
        }
        /// <summary>
        /// 获取Json 字符串类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
