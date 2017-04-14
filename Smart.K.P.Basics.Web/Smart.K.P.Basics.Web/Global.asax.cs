using SmartCommon.LogHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Smart.K.P.Basics.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var ex = new Exception();
            if (Context != null)
            {
                ex = Context.Request.RequestContext.HttpContext.Error;
                LogService.WriteErrorLog(ex, extContent: "未处理的错误信息");
                Response.Redirect("Error.html");
            }
        }
    }
}
