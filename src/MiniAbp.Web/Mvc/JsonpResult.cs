using System;
using System.Web.Mvc;
using MiniAbp.Extension;

namespace Sl.Bpm.Basic.Mvc.Result
{
    public class JsonpResult : JsonResult
    {
        public static readonly string JsonpCallbackName = "callback";
        public static readonly string CallbackApplicationType = "application/json";

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if ((JsonRequestBehavior == JsonRequestBehavior.DenyGet) &&
                  string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException();
            }
            var response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(ContentType))
                response.ContentType = ContentType;
            else
                response.ContentType = CallbackApplicationType;
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data != null)
            {
                var request = context.HttpContext.Request;
                string buffer = request[JsonpCallbackName] != null ? 
                    string.Format("{0}({1})", request[JsonpCallbackName], Data.SerializeJson()) :
                    Data.SerializeJson();
                response.Write(buffer);
            }
        }
    }
}
