using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using YesHJ.Log.Business;
using YesHJ.Log.Model;

namespace YesHJ.Log.Web.Admin
{
    public partial class JobList : BasePage
    {
        protected string JobServiceName = string.Empty;
        protected string SiteName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "POST")
                Update();
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                Query();

            string siteId = Request.QueryString["siteId"] + string.Empty;
            if (!string.IsNullOrEmpty(siteId))
            {
                int siteid;
                if (int.TryParse(siteId, out siteid))
                    SiteName = SiteBusiness.GetSiteInfoById(siteid).SiteName;
            }
            else
                SiteName = Request.QueryString["sitename"] + string.Empty;

            string jobserviceId = Request.QueryString["jobserviceId"] + string.Empty;
            if (!string.IsNullOrEmpty(jobserviceId))
            {
                int jobserviceid;
                if (int.TryParse(jobserviceId, out jobserviceid))
                    JobServiceName =
                        JobServiceBusiness.GetJobServiceInfoById(Convert.ToInt32(jobserviceId)).JobServiceName;
            }
            else
                JobServiceName = Request.QueryString["jobservicename"] + string.Empty;


            var selectsite = new List<string> {"All"};
            var selectjobservice = new List<string> {"All"};
            SiteName = string.IsNullOrEmpty(SiteName) ? "All" : SiteName;
            JobServiceName = string.IsNullOrEmpty(JobServiceName) ? "All" : JobServiceName;

            //填充site select
            Func<SiteInfo, bool> func = s => true;
            if (!CurrentUser.IsManager)
            {
                func = site => UserSites.Exists(usersitemapping => usersitemapping.SiteId == site.Id);
            }
            IEnumerable<SiteInfo> sites = SiteBusiness.GetSiteInfos().Where(func); //普通用户所拥有的站点
            SiteInfo[] siteInfos = sites as SiteInfo[] ?? sites.ToArray();
            selectsite.AddRange(siteInfos.Select(sitetemp => sitetemp.SiteName).ToList());
            //填充jobservice select
            selectjobservice.AddRange(
                JobServiceBusiness.GetJobServiceInfos().Select(jobservicetemp => jobservicetemp.JobServiceName).ToList());

            List<JobInfo> jobList = JobBusiness.GetJobInfos();

            List<int> siteIds = siteInfos.Select(sitetemp => sitetemp.Id).ToList();

            jobList = SiteName == "All"
                ? jobList.Where(joblisttemp => siteIds.Contains(joblisttemp.SiteId)).ToList()
                : jobList.Where(joblisttemp => joblisttemp.SiteId == SiteBusiness.GetSiteInfoByName(SiteName).Id)
                    .ToList();
            if (JobServiceName != "All")
            {
                jobList =
                    jobList.Where(
                        joblisttemp =>
                            joblisttemp.JobServiceId == JobServiceBusiness.GetJobServiceInfoByName(JobServiceName).Id)
                        .ToList();
            }
            rep_main.DataSource = jobList.OrderByDescending(s => s.Id);
            rep_main.DataBind();

            hid_sitename.Value = string.Join(",", selectsite.ToArray()) + "|" + SiteName;
            hid_jobservicename.Value = string.Join(",", selectjobservice.ToArray()) + "|" + JobServiceName;
        }

        private void Query()
        {
            Response.ContentType = "application/json";
            int id = Convert.ToInt32(Request.QueryString["id"]);
            JobInfo jobInfo = JobBusiness.GetJobInfoById(id);
            var jobentity = new JobEntity
            {
                Id = jobInfo.Id,
                JobServiceName = JobServiceBusiness.GetJobServiceInfoById(jobInfo.JobServiceId).JobServiceName,
                SiteName = SiteBusiness.GetSiteInfoById(jobInfo.SiteId).SiteName,
                JobStatus = jobInfo.JobStatus,
                JobThreadCount = jobInfo.JobThreadCount
            };
            Response.Write(new JavaScriptSerializer().Serialize((new {jobentity})));
            Response.End();
        }

        private void Update()
        {
            if (Request.Form.AllKeys.Contains("id")) //edit
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                JobInfo jobInfo = JobBusiness.GetJobInfoById(id);
                if (jobInfo != null)
                {
                    jobInfo.JobStatus = Request.Form.AllKeys.Contains("JobStatus");
                    string jobThreadCount = Request.Form["JobThreadCount"];
                    int count;
                    if (!int.TryParse(jobThreadCount, out count))
                    {
                        Response.Write("CountFormatError");
                        Response.End();
                    }
                    jobInfo.JobThreadCount = count;
                    JobBusiness.Update(jobInfo);
                }
            }
            Response.Write("ok");
            Response.End();
        }

        protected string GetJobServiceName(object jobserviceId)
        {
            return JobServiceBusiness.GetJobServiceInfoById(Convert.ToInt32(jobserviceId)).JobServiceName;
        }

        protected string GetSiteName(object siteId)
        {
            return SiteBusiness.GetSiteInfoById(Convert.ToInt32(siteId)).SiteName;
        }

        private class JobEntity
        {
            public int Id { get; set; }
            public string JobServiceName { get; set; }
            public string SiteName { get; set; }
            public bool JobStatus { get; set; }
            public int JobThreadCount { get; set; }
        }
    }
}