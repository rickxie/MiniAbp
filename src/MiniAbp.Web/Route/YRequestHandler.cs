using System;
using MiniAbp.Dependency;
using MiniAbp.Logging;
using MiniAbp.Web.Auditing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MiniAbp.Web.Route
{
    public class YRequestHandler 
    {
        private static readonly ILogger Logger = IocManager.Instance.Resolve<ILogger>();

        public static string ApiService(string service, string method, object param)
        {
            var auditing = new AuditingManager();
            AjaxResult result = null;
            auditing.Start(service, method, param.ToString());
            try
            {
                var dataResult = ServiceController.Instance.Execute(service, method, param, RequestType.ServiceFile);
                result = new AjaxResult()
                {
                    IsAuthorized = true,
                    IsSuccess = true,
                    Result = dataResult
                };
            }
            catch (Exception ex)
            {
                var except = ex.InnerException ?? ex;

                result = new AjaxResult()
                {
                    IsAuthorized = true,
                    IsSuccess = false,
                    Result = null,
                    Errors = new Errors()
                    {
                        Message = except.Message,
                        //CallStack = except.StackTrace
                    }
                };


                if (except.GetType() == typeof (UserFriendlyException))
                {
                    result.Errors.IsFriendlyError = true;
                }
                else if (except.GetType() == typeof (AuthorizationException))
                {
                    result.Errors.IsFriendlyError = false;
                    result.IsAuthorized = false;
                }
                if (except.GetType() == typeof (UserFriendlyException))
                {
                    var exc = except as UserFriendlyException;
                    var newExc = exc?.InnerException ?? exc;
                    auditing.Exception(newExc?.Message + newExc?.StackTrace);
                    try
                    {
                        Logger.Error(newExc?.Message, newExc);
                    }
                    catch (Exception)
                    {
                        auditing._auditInfo.Exception += "Logs文件夹没有权限。";
                    }
                }
                else
                {
                    auditing.Exception(ex.Message + ex.StackTrace);
                    try
                    {
                        Logger.Error(ex.Message, ex);
                    }
                    catch (Exception)
                    {
                        auditing._auditInfo.Exception += "Logs文件夹没有权限。";
                    }
                }
               
            }
            var responseStr = JsonConvert.SerializeObject(result, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            auditing.Stop(responseStr);
            return responseStr;
        }

    }
}