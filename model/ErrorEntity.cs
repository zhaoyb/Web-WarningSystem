using System;
using System.Collections.Specialized;

namespace model
{
    public class ErrorEntity
    {
        public string Id { get; set; }
        public string WebToken { get; set; }
        public string MachineName { get; set; }
        public string Ip { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionSource { get; set; }
        public string ExceptionDetail { get; set; }
        public int HttpStatusCode { get; set; }
        public string RequestUrl { get; set; }
        public string ServerVariables { get; set; }
        public string QueryString { get; set; }
        public string Form { get; set; }
        public string Cookies { get; set; }
        public DateTime DateTime { get; set; }
        public int NotifyStatus { get; set; }   //通知状态   0  未通知，  1 已通知
    }
}
