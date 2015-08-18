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
        private string _clientkey = string.Empty;

        private readonly WebSiteBusiness _webSiteBusiness = new WebSiteBusiness();

        protected void Page_Load(object sender, EventArgs e)
        {
            _clientkey = Guid.NewGuid().ToString("N");
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
                model.WebSite webSite = _webSiteBusiness.GetSingleOrDefault(string.Format("select * from WebSite where Id={0}", id));
                Response.Write(new JavaScriptSerializer().Serialize(new { webSite }));
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
                    if (Request.Form.AllKeys.Contains("delete")) //delete       //考虑变更为分布式事务
                    {
                        foreach (JobInfo jobInfo in JobBusiness.GetJobInfoBySiteId(id))  //删除job
                        {
                            JobBusiness.Delete(jobInfo);
                        }

                        foreach (UserSiteMapping siteMapping in UserSiteMappingBusiness.GetUserSiteMappingBySiteId(id))  //删除mapping信息
                        {
                            UserSiteMappingBusiness.Delete(siteMapping);
                        }
                        SiteBusiness.Delete(site);
                    }
                    else
                    {
                        webSite.SiteName = Request.Form["SiteName"];
                        webSite.SiteUrl = Request.Form["SiteUrl"];

                        bool sitestatus = Request.Form.AllKeys.Contains("SiteStatus");
                        if (webSite.SiteStatus != sitestatus) //site status调整，job 线程状态跟着调整
                        {
                            foreach (JobInfo jobinfo in JobBusiness.GetJobInfoBySiteId(id))
                            {
                                jobinfo.JobStatus = sitestatus;
                                JobBusiness.Update(jobinfo);
                            }
                        }
                        webSite.SiteStatus = sitestatus;
                        webSite.SiteToken = Request.Form["SiteToken"];

                        SiteBusiness.Update(site);
                    }
                }
            }
            else
            {
                webSite = new  model.WebSite()
                {
                    SiteName = Request.Form["SiteName"],
                    SiteUrl = Request.Form["SiteUrl"],
                    SiteStatus = Request.Form.AllKeys.Contains("SiteStatus"),
                    SiteToken = Request.Form["SiteToken"],
                    SiteLevel = Request.Form["SiteLevel"],
                    SiteAuthor = CurrentUser.RealName
                };
                string backupInterval = Request.Form["SiteBackUpInterval"];
                int initerval;
                if (!int.TryParse(backupInterval, out initerval))
                {
                    Response.Write("IntervalFormatError");
                    Response.End();
                }
                site.SiteBackUpInterval = initerval;
                site.SiteBackUpIntervalUnit = Request.Form["SiteBackUpIntervalUnit"];
                string backupTime = Request.Form["SiteNextBackUpTime"];
                DateTime time;
                if (!DateTime.TryParse(backupTime, out time))
                {
                    Response.Write("TimeFormatError");
                    Response.End();
                }
                site.SiteNextBackUpTime = backupTime;
                if (SiteBusiness.GetSiteInfos().FirstOrDefault(s => s.SiteName.ToLower() == site.SiteName.ToLower()) != null)
                {
                    Response.Write("exists");
                    Response.End();
                }
                if (site.SiteName.ToLower() == "all")
                {
                    Response.Write("keyword");
                    Response.End();
                }
                SiteBusiness.Save(site);
                UserSiteMappingBusiness.Save(new UserSiteMapping
                {
                    SiteId = site.Id,
                    UserId = CurrentUser.Id
                });

                foreach (JobServiceInfo item in JobServiceBusiness.GetJobServiceInfos()) //新增site ，也就在job中新增site
                {
                    JobBusiness.Save(new JobInfo
                    {
                        JobServiceId = item.Id,
                        JobThreadCount = GetThreadCountByLevel(site.SiteLevel),
                        JobStatus = site.SiteStatus,
                        SiteId = site.Id
                    });
                }
            }

            Response.Write("ok");
            Response.End();
        }
    }
}