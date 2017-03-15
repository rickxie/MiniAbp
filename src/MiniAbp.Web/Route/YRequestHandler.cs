using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Runtime.Serialization;
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

        public static AjaxResult ApiService(string service, string method, object param)
        {
            AjaxResult result = null;
            try
            {
                var dataResult = ServiceController.Instance.Execute(service, method, param);
                result = new AjaxResult()
                {
                    IsAuthorized = true,
                    IsSuccess = true,
                    Result = dataResult
                };
            }
            catch (Exception ex)
            {
                var except = GetInnerException(ex);

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
                    //result.e
                    result.Exception = newExc?.Message + newExc?.StackTrace;
                    try
                    {
                        Logger.Error(newExc?.Message, newExc);
                    }
                    catch (Exception)
                    {
                        result.Exception += "Logs文件夹没有权限。";
                    }
                }
                else
                {
                    result.Exception = ex.Message + ex.StackTrace + ex.InnerException?.Message;
                    try
                    {
                        Logger.Error(ex.Message, ex);
                    }
                    catch (Exception)
                    {
                        result.Exception += "Logs文件夹没有权限。";
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 获取内部异常信息
        /// </summary>
        /// <returns></returns>
        private static Exception GetInnerException(Exception ex)
        {
            Exception exce = null;
            if (ex.InnerException != null)
                exce = GetInnerException(ex.InnerException);
            else
            {
                exce = ex;
            }
            return exce;
        }
    }
}