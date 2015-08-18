using model;
using System.Collections.Generic;

namespace business
{
    public class WebServerBusiness : BaseBusiness<WebServer>
    {
        public WebServerBusiness()
            : base("WebServer", "Id")
        {
        }
    }
}