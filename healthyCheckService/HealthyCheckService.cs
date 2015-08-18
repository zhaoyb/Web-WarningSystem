using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using business;
using FluentScheduler;
using model;
using utility;
using warning;

namespace webcheckservice
{
    public partial class HealthyCheckService : ServiceBase
    {

        private readonly WebSiteBusiness _webSiteBusiness = new WebSiteBusiness();

        private readonly WebServerBusiness _webServerBusiness = new WebServerBusiness();
        public HealthyCheckService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TaskManager.AddTask(Handle, x => x.WithName("HealthyCheckService").NonReentrant().ToRunEvery(10).Seconds());
        }

        protected override void OnStop()
        {
            TaskManager.GetSchedule("HealthyCheckService").Disable();
        }

        private  void Handle()
        {
            //获取所有的网站， 依次检查返回的是否为ok, 或者检查http.status=200
            IEnumerable<WebSite> webSites= _webSiteBusiness.GetList("select * from WebSite");
            Parallel.ForEach(webSites, website =>
            {
                IEnumerable<WebServer> webServers =
                    _webServerBusiness.GetList(
                        (string.Format("select * from WebServer where WebToken='{0}'", website.WebToken)));

                Parallel.ForEach(webServers, webserver =>
                {
                    //这里如果你仅仅想获取状态码， 可以调用 HttpHelper.GetHttpStatus()
                   var responseText= HttpHelper.SendGetRequest("http://" + webserver.ServerIp + "", "", website.WebUrl, 3000,
                        Encoding.UTF8);

                    if (responseText != "ok")
                    {
                        //站点出了异常，需要发送到服务端进行处理
                        ClientErrorEntity clientErrorEntity = new ClientErrorEntity();
                        clientErrorEntity.ExceptionMessage = website.WebName + "(" + "http://" + webserver.ServerIp + "" + ")" + "访问发生异常";
                        ClientErrorEntity.WebToken = website.WebToken;   //重新设置Token

                        SendErrorEntity.SendError(clientErrorEntity);
                    }
                });

            });
        }
    }
}
