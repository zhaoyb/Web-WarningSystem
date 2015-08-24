using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using business;
using dto;

namespace admin
{
    /// <summary>
    /// Report 的摘要说明
    /// </summary>
    public class Report : IHttpHandler
    {
        string sql = @"select count(ErrorEntity.id) Cnt,datepart(hh,DateTime) [Hour]
                             from ErrorEntity inner join WebSite on WebSite.WebToken = ErrorEntity.WebToken
                             where WebSite.WebName='{2}' and DateTime between '{0}' and '{1}' 
                             group by datepart(hh,DateTime)";

        private ReportBusiness _reportBusiness = new ReportBusiness();


        public void ProcessRequest(HttpContext context)
        {
             var count = new int[24];
            string siteName = context.Request.QueryString["WebName"] ?? "";
            if (siteName != "")
            {
                IEnumerable<ReportDto> reportDtos =
                    _reportBusiness.GetList(string.Format(sql, DateTime.Now.ToString("yyyy-MM-dd 00:00:00"),
                        DateTime.Now.ToString("yyyy-MM-dd 23:59:59"), siteName));

                foreach (ReportDto reportDto in reportDtos)
                {
                    count[reportDto.Hour] = reportDto.Cnt;
                }
            }
            context.Response.Write( string.Join(",", count));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}