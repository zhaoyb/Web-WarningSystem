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



        private Dictionary<int, int> count = new Dictionary<int, int>()
        {
            {0, 0},
            {1, 0},
            {2, 0},
            {3, 0},
            {4, 0},
            {5, 0},
            {6, 0},
            {7, 0},
            {8, 0},
            {9, 0},
            {10, 0},
            {11, 0},
            {12, 0},
            {13, 0},
            {14, 0},
            {15, 0},
            {16, 0},
            {17, 0},
            {18, 0},
            {19, 0},
            {20, 0},
            {21, 0},
            {22, 0},
            {23, 0},
        };

        string sql = @"select count(ErrorEntity.id) Cnt,datepart(hh,DateTime) [Hour]
                             from ErrorEntity inner join WebSite on WebSite.WebToken = ErrorEntity.WebToken
                             where WebSite.WebName='{2}' and DateTime between '{0}' and '{1}' 
                             group by datepart(hh,DateTime)";

        private ReportBusiness _reportBusiness = new ReportBusiness();


        public void ProcessRequest(HttpContext context)
        {
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
            context.Response.Write( string.Join(",", count.Values.ToArray()));
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