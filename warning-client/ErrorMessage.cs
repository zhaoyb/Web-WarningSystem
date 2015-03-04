using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Web;

namespace warning
{
    public class ErrorMessage
    {
        public string MachineName { get; set; }
        public string Ip { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionSource { get; set; }
        public string ExceptionDetail { get; set; }
        public int HttpStatusCode { get; set; }
        public string HttpHostHtmlMessage { get; set; }
        public string RequestUrl { get; set; }
        public NameValueCollection ServerVariables { get; set; }
        public NameValueCollection QueryString { get; set; }
        public NameValueCollection Form { get; set; }
        public NameValueCollection Cookies { get; set; }
        public DateTime DateTime { get; set; }
        public string Remark { get; set; }

        public ErrorMessage(Exception exception, HttpContext httpContext)
        {
            MachineName = Environment.MachineName;
            Ip = Utility.GetLocalIp();
            ExceptionType = exception.GetType().FullName;
            ExceptionMessage = exception.Message;
            ExceptionSource = exception.Source;
            ExceptionDetail = exception.ToString();
            DateTime = DateTime.Now;
            HttpException httpException = exception as HttpException;
            if (httpException != null)
            {
                HttpStatusCode = httpException.GetHttpCode();
                HttpHostHtmlMessage =httpException.GetHtmlErrorMessage();
            }

            if (httpContext != null)
            {
                HttpRequest request = httpContext.Request;
                RequestUrl = request.Url.AbsoluteUri;
                ServerVariables = Utility.CopyCollection(request.ServerVariables);
                QueryString = Utility.CopyCollection(request.QueryString);
                Form = Utility.CopyCollection(request.Form);
                Cookies = Utility.CopyCollection(request.Cookies);
            }
        }
      
    }
}
