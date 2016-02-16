using System;
using MiniAbp.Dependency;
using MiniAbp.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Yooya.Bpm.Framework.Route;

namespace MiniAbp.Route
{
    public class YRequestHandler 
    {
        private static readonly ILogger Logger = IocManager.Instance.Resolve<ILogger>();
        public static string ApiService(string service, string method, object param)
        {
            object result = null;
            try
            {
                result = ServiceController.Instance.Execute(service, method, param, RequestType.ServiceFile);
                result = new AjaxResult()
                {
                    IsSuccess = true,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                var except = ex.InnerException ?? ex;
                if (except.GetType() == typeof (UserFriendlyException))
                {
                    result = new AjaxResult()
                    {
                        IsSuccess = false,
                        Result = null,
                        Errors = new Errors()
                        {
                            Message = except.Message,
                            CallStack = except.StackTrace
                        }
                    };
                }
                Logger.Error(ex.Message, except);
            }
            return JsonConvert.SerializeObject(result, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

    }
}