using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Web;
using Newtonsoft.Json;

namespace warning
{
    public class ClientErrorEntity
    {
        [JsonProperty]
        public static string WebToken { get; set; }


        public string Id { get; set; }
     
        public string MachineName { get; set; }
        public string Ip { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionSource { get; set; }
        public string ExceptionDetail { get; set; }
        public int HttpStatusCode { get; set; }
        public string RequestUrl { get; set; }
        public Dictionary<string,string> ServerVariables { get; set; }
        public Dictionary<string, string> QueryString { get; set; }
        public Dictionary<string, string> Form { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
        public DateTime DateTime { get; set; }
        public int Type { get; set; }   //异常类型， 0 系统异常，  1 监测访问异常

        static ClientErrorEntity()
        {
            IDictionary webWarningSetting = ConfigurationManager.GetSection("WebWarningSetting") as IDictionary;
            if (webWarningSetting != null)
            {
                WebToken = webWarningSetting["WebToken"].ToString();
            }
        }

        public ClientErrorEntity()
        {
            Id = Guid.NewGuid().ToString("N");
            DateTime = DateTime.Now;
        }

        public ClientErrorEntity(Exception exception, HttpContext httpContext)
        {
            Id = Guid.NewGuid().ToString("N");

            MachineName = Environment.MachineName;
            Ip = Common.GetLocalIp();
            ExceptionType = exception.GetType().FullName;
            ExceptionMessage = exception.Message;
            ExceptionSource = exception.Source;
            ExceptionDetail = exception.ToString();
            DateTime = DateTime.Now;
            HttpException httpException = exception as HttpException;
            if (httpException != null)
            {
                HttpStatusCode = httpException.GetHttpCode();
            }

            if (httpContext != null)
            {
                HttpRequest request = httpContext.Request;
                RequestUrl = request.Url.AbsoluteUri;
                ServerVariables = Common.ConvertCollectionToDictionary(request.ServerVariables);
                QueryString = Common.ConvertCollectionToDictionary(request.QueryString);
                Form = Common.ConvertCollectionToDictionary(request.Form);
                Cookies = Common.ConvertCollectionToDictionary(request.Cookies);
            }
            Type = 0;
        }
      
    }
}
