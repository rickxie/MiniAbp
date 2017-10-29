using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sl.Bpm.Basic.Mvc.Result;

namespace MiniAbp.Web.Mvc
{
    public static class ControllerExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JsonpResult Jsonp(this Controller controller, object data)
        {
            JsonpResult result = new JsonpResult()
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
        }
        /// <summary>
        /// 是否PC
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool IsPcBrowser(this Controller controller)
        {
            bool b = true;
            string agent = controller.Request.UserAgent;
            string[] agents = { "Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod" };
            for (int i = 0; i < agents.Length; i++)
            {
                if (agent.IndexOf(agents[i]) > -1)
                {
                    b = false;
                }
            }
            return b;
        }


        /// <summary>
        ///  输出编译后的Html, this.RenderView("~/Views/Home/_Header.cshtml", this.ViewData, this.TempData);
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="cc"></param>
        /// <param name="tempUrl"></param>
        /// <param name="vd"></param>
        /// <param name="td"></param>
        /// <returns></returns>
        public static string RenderView(this Controller ctrl, string tempUrl, ViewDataDictionary vd, TempDataDictionary td)
        {
            ControllerContext cc = ctrl.ControllerContext;
            string html = string.Empty;
            IView v = ViewEngines.Engines.FindView(cc, tempUrl, "").View;
            using (StringWriter sw = new StringWriter())
            {
                ViewContext vc = new ViewContext(cc, v, vd, td, sw);
                vc.View.Render(vc, sw);
                html = sw.ToString();
            }
            return html;
        }
    }
}
