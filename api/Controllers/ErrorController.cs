using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using dto;
using utility;

namespace api.Controllers
{
    public class ErrorController : ApiController
    {
        public string Post([FromBody]ErrorMessage errorMessage)
        {
            if (errorMessage != null && !string.IsNullOrWhiteSpace(errorMessage.Id))
            {
                if (RedisHelper.EnqueueItemOnList("ErrorMessageQueue", errorMessage.Id))
                {
                    if (RedisHelper.Set(errorMessage.Id, errorMessage))
                    {
                        return "ok";
                    }
                    return "add set error";
                }
                return "add item to list error";
            }
            return "modle error";
        }
    }
}
