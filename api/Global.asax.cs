using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using utility;

namespace api
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RedisHelper.Init();
        }
    }
}