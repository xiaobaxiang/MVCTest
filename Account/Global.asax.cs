using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Account
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(Object sender, EventArgs e)//会话中如果应用程序出现错误会执行这个
        {
            Exception lastError = Server.GetLastError();
            lastError.LogError();
            Response.Redirect("~/Error.html");
        }
    }
}
