using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using YesHJ.Log.Web.Admin;

namespace admin
{
    public partial class WebSite : BasePage
    {
        protected string WebToken = string.Empty;
        protected string Keyword = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            WebToken = Guid.NewGuid().ToString("N");
            Keyword = Request.QueryString["keyword"] + string.Empty;

            if (Request.HttpMethod == "POST")
            {
                Update();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                Query();

            Func<SiteInfo, bool> func = s => true;
            if (!CurrentUser.IsManager)
            {
                func = site => UserSites.Exists(usersitemapping => usersitemapping.SiteId == site.Id);
            }
            IEnumerable<SiteInfo> sites = SiteBusiness.GetSiteInfos().Where(func);
            if (!string.IsNullOrEmpty(Keyword))
            {
                sites =
                    sites.Where(
                        site =>
                            site.SiteName.ToLower().Contains(Keyword) || site.SiteUrl.ToLower().Contains(Keyword) ||
                            site.SiteToken == Keyword);
            }
            rep_main.DataSource = sites.OrderByDescending(site => site.Id);
            rep_main.DataBind();
        }

        private void Query()
        {
            Response.ContentType = "application/json";
            int id;
            if (int.TryParse(Request.QueryString["id"], out id))
            {
                SiteInfo data = SiteBusiness.GetSiteInfoById(id);
                Response.Write(new JavaScriptSerializer().Serialize(new { data }));
                Response.End();
            }
        }

        private void Update()
        {
            SiteInfo site;
            if (Request.Form.AllKeys.Contains("id") && HasPermission(Convert.ToInt32(Request.Form["id"]))) //edit
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                site = SiteBusiness.GetSiteInfoById(id);
                if (site != null)
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
                        site.SiteName = Request.Form["SiteName"];
                        site.SiteUrl = Request.Form["SiteUrl"];

                        bool sitestatus = Request.Form.AllKeys.Contains("SiteStatus");
                        if (site.SiteStatus != sitestatus) //site status调整，job 线程状态跟着调整
                        {
                            foreach (JobInfo jobinfo in JobBusiness.GetJobInfoBySiteId(id))
                            {
                                jobinfo.JobStatus = sitestatus;
                                JobBusiness.Update(jobinfo);
                            }
                        }
                        site.SiteStatus = sitestatus;
                        site.SiteToken = Request.Form["SiteToken"];
                        site.SiteAuthor = Request.Form["SiteAuthor"];

                        string sitelevel = Request.Form["SiteLevel"];
                        if (site.SiteLevel != sitelevel) //site level调整，job 线程数跟着调整
                        {
                            foreach (JobInfo jobInfo in JobBusiness.GetJobInfoBySiteId(id))
                            {
                                jobInfo.JobThreadCount = GetThreadCountByLevel(sitelevel);
                                JobBusiness.Update(jobInfo);
                            }
                        }
                        site.SiteLevel = sitelevel;
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
                        if (time < DateTime.Now)
                        {
                            Response.Write("TimeError");
                            Response.End();
                        }
                        site.SiteNextBackUpTime = backupTime;
                        SiteBusiness.Update(site);
                    }
                }
            }
            else
            {
                site = new SiteInfo
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


                //新建mongodb集合

                MongoDbManager.CreateCollection("", site.SiteName);
                MongoDbManager.EnsureIndex(new[] { "TimeStamp" }, "", site.SiteName);

                //后续可以考虑新增rabbitmq


            }

            Response.Write("ok");
            Response.End();
        }

        private int GetThreadCountByLevel(string level)
        {
            switch (level)
            {
                case "高":
                    return 3;
                case "中":
                    return 2;
                case "低":
                    return 1;
            }
            return 0;
        }
    }
}