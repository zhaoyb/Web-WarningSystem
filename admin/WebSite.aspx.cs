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
    public partial class WebSite : Page
    {
        public string Clientkey = string.Empty;

        private readonly WebSiteBusiness _webSiteBusiness = new WebSiteBusiness();

        private readonly WebServerBusiness _webServerBusiness = new WebServerBusiness();

        protected void Page_Load(object sender, EventArgs e)
        {
            Clientkey = Guid.NewGuid().ToString("N");
            if (Request.HttpMethod == "POST")
            {
                Update();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                Query();

            rep_main.DataSource = _webSiteBusiness.GetList("select * from WebSite");
            rep_main.DataBind();

        }

        private void Query()
        {
            Response.ContentType = "application/json";
            int id;
            if (int.TryParse(Request.QueryString["id"], out id))
            {
                model.WebSite data = _webSiteBusiness.GetSingleOrDefault(string.Format("select * from WebSite where Id={0}", id));
                Response.Write(new JavaScriptSerializer().Serialize(new { data }));
                Response.End();
            }
        }

        private void Update()
        {
            model.WebSite webSite;
            if (Request.Form.AllKeys.Contains("id")) //edit
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                webSite = _webSiteBusiness.GetSingleOrDefault(string.Format("select * from WebSite where Id={0}", id));
                if (webSite != null)
                {
                    if (Request.Form.AllKeys.Contains("delete")) //delete      
                    {
                        //1 删除站点下的服务器
                        _webServerBusiness.DeleteBySql(string.Format("where WebId = {0}", id));
                        //2 删除站点信息
                        _webSiteBusiness.Delete(webSite);
                    }
                    else
                    {
                        webSite.WebName = Request.Form["WebName"];
                        webSite.Host = Request.Form["Host"];
                        webSite.Manager = Request.Form["Manager"];
                        webSite.ManagerPhone = Request.Form["ManagerPhone"];
                        webSite.ManagerEmail = Request.Form["ManagerEmail"];
                        webSite.CheckUrl = Request.Form["CheckUrl"];


                        bool existsEnable = Request.Form.AllKeys.Contains("Enable");
                        webSite.Enable = existsEnable ? 1 : 0;
                        _webSiteBusiness.Update(webSite);
                    }
                }
            }
            else
            {
                string webName = Request.Form["WebName"] ?? "";
                webSite = _webSiteBusiness.GetSingleOrDefault(string.Format("select * from WebSite where WebName='{0}'", webName));

                if (webSite != null)
                {
                    Response.Write("exists");
                    Response.End();
                }
                else
                {
                    webSite = new model.WebSite()
                    {
                        WebName = Request.Form["WebName"],
                        Host = Request.Form["Host"],
                        WebToken = Request.Form["WebToken"],
                        Manager = Request.Form["Manager"],
                        ManagerPhone = Request.Form["ManagerPhone"],
                        ManagerEmail = Request.Form["ManagerEmail"],
                        CheckUrl = Request.Form["CheckUrl"],
                        Enable = Request.Form.AllKeys.Contains("Enable") ? 1 : 0
                    };
                    _webSiteBusiness.Insert(webSite);
                }
            }
            Response.Write("ok");
            Response.End();
        }
    }
}