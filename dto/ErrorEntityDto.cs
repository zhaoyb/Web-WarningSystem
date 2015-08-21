using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace dto
{
    public class ErrorEntityDto
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
        public Dictionary<string, string> ServerVariables { get; set; }
        public Dictionary<string, string> QueryString { get; set; }
        public Dictionary<string, string> Form { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
        public DateTime DateTime { get; set; }
        public int Type { get; set; }
    }
}
