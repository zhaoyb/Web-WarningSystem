using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using dto;

namespace api.Controllers
{
    public class ErrorController : ApiController
    {
        public void Post([FromBody]ExceptionMessage exceptionMessage)
        {

        }
    }
}
