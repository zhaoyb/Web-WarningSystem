using System;
using System.Collections.Specialized;

namespace dto
{
    public class ErrorEntity
    {
        public string Id { get; set; }
        public string WebToekn { get; set; }
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
    }
}
