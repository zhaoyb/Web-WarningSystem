using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using YesHJ.Log.Business;
using YesHJ.Log.Model;

namespace YesHJ.Log.Web.Admin
{
    public partial class JobService : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "POST")
            {
                Update();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                Query();

            List<JobServiceInfo> jobservice = JobServiceBusiness.GetJobServiceInfos();
            rep_main.DataSource = jobservice.OrderByDescending(jobserviceTemp => jobserviceTemp.Id);
            rep_main.DataBind();
        }

        private void Query()
        {
            Response.ContentType = "application/json";
            int id;
            if (int.TryParse(Request.QueryString["id"], out id))
            {
                JobServiceInfo data = JobServiceBusiness.GetJobServiceInfoById(id);
                Response.Write(new JavaScriptSerializer().Serialize(new { data }));
                Response.End();
            }
        }

        private void Update()
        {
            JobServiceInfo jobServiceInfo;
            if (Request.Form.AllKeys.Contains("id") && HasPermission(Convert.ToInt32(Request.Form["id"]))) //edit
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                jobServiceInfo = JobServiceBusiness.GetJobServiceInfoById(id);
                if (jobServiceInfo != null)
                {
                    if (Request.Form.AllKeys.Contains("delete")) //delete
                    {
                        foreach (JobInfo jobInfo in JobBusiness.GetJobInfosByServiceId(id)) //删除job信息         //这里所有的操作都要是事务性的，可考虑使用分布式事务
                        {
                            JobBusiness.Delete(jobInfo);
                        }
                        JobServiceBusiness.Delete(jobServiceInfo); //删除job服务器信息
                    }
                    else
                    {
                        jobServiceInfo.JobServiceIp = Request.Form["JobServiceIp"];
                        jobServiceInfo.JobServiceName = Request.Form["JobServiceName"];
                        bool jobServiceStatus = Request.Form.AllKeys.Contains("JobServiceStatus");
                        if (jobServiceInfo.JobServiceStatus != jobServiceStatus)
                        {
                            foreach (JobInfo jobInfo in JobBusiness.GetJobInfosByServiceId(id)) //往job中添加数据site job
                            {
                                jobInfo.JobStatus = jobServiceStatus;
                                JobBusiness.Update(jobInfo);
                            }
                        }
                        jobServiceInfo.JobServiceStatus = jobServiceStatus;
                        JobServiceBusiness.Update(jobServiceInfo);
                    }
                }
            }
            else
            {
                jobServiceInfo = new JobServiceInfo
                {
                    JobServiceIp = Request.Form["JobServiceIp"],
                    JobServiceName = Request.Form["JobServiceName"],
                    JobServiceStatus = Request.Form.AllKeys.Contains("JobServiceStatus")
                };

                if (
                    JobServiceBusiness.GetJobServiceInfos()
                        .FirstOrDefault(
                            jobservice => jobservice.JobServiceIp.ToLower() == jobServiceInfo.JobServiceIp.ToLower()) !=
                    null)
                {
                    Response.Write("exists");
                    Response.End();
                }
                if (jobServiceInfo.JobServiceName.ToLower() == "all")
                {
                    Response.Write("keyword");
                    Response.End();
                }
                JobServiceBusiness.Save(jobServiceInfo);
                foreach (SiteInfo item in SiteBusiness.GetSiteInfos()) //往job中添加数据site job
                {
                    JobBusiness.Save(new JobInfo
                    {
                        JobServiceId = jobServiceInfo.Id,
                        SiteId = item.Id,
                        JobStatus = jobServiceInfo.JobServiceStatus,
                        JobThreadCount = GetThreadCountByLevel(item.SiteLevel)
                    });
                }
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