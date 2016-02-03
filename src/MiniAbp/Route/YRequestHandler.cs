using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Yooya.Bpm.Framework.Route;

namespace MiniAbp.Route
{
    public class YRequestHandler 
    {
        public static string ApiService(string service, string method, object param)
        {
          
            var result = ServiceController.Instance.Execute(service, method, param, RequestType.ServiceFile);
            return JsonConvert.SerializeObject(result, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

    }
}