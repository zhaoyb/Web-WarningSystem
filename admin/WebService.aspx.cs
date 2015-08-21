using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Web.UI;
using business;

namespace admin
{
    public partial class WebService : Page
    {

        private readonly WebServerBusiness _webServerBusiness = new WebServerBusiness();

        public int WebId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "POST")
            {
                Update();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                Query();

            WebId = Convert.ToInt32(Request.QueryString["WebId"]);
            rep_main.DataSource = _webServerBusiness.GetList("select * from WebServer where WebId = " + WebId);
            rep_main.DataBind();

        }

        private void Query()
        {
            Response.ContentType = "application/json";
            int id;
            if (int.TryParse(Request.QueryString["id"], out id))
            {
                model.WebServer data = _webServerBusiness.GetSingleOrDefault(string.Format("select * from WebServer where Id={0}", id));
                Response.Write(new JavaScriptSerializer().Serialize(new { data }));
                Response.End();
            }
        }

        private void Update()
        {
            model.WebServer webServer;
            if (Request.Form.AllKeys.Contains("id")) //edit
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                webServer = _webServerBusiness.GetSingleOrDefault(string.Format("select * from WebServer where Id={0}", id));
                if (webServer != null)
                {
                    if (Request.Form.AllKeys.Contains("delete")) //delete      
                    {
                        _webServerBusiness.Delete(webServer);
                    }
                    else
                    {
                        webServer.ServerIp = Request.Form["ServerIp"];
                        bool existsEnable = Request.Form.AllKeys.Contains("Enable");
                        webServer.Enable = existsEnable ? 1 : 0;
                        _webServerBusiness.Update(webServer);
                    }
                }
            }
            else
            {
                webServer = new model.WebServer()
                {
                    ServerIp = Request.Form["ServerIp"],
                    WebId =Convert.ToInt32( Request.Form["WebId"]),
                    Enable = Request.Form.AllKeys.Contains("Enable") ? 1 : 0
                };
                _webServerBusiness.Insert(webServer);
            }
            Response.Write("ok");
            Response.End();
        }
    }
}