using System.Net;
using System.Net.Http;
using System.Text;
using dto;
using System.Web.Http;
using utility;

namespace api.Controllers
{
    public class ErrorController : ApiController
    {
        /// <summary>
        /// 1. Redis的list可以模拟队列  异常信息都会发送到队列中进行暂存
        /// 2. 对于异常信息， 会发送到Redis List中，  但是Redis的List的Value并不支持复杂结构的对象， 所以这里采用类似于2级指针的方法， 将ErrorEntity.Id放入到队列中， ErrorEntity则以普通的方式set到Redis中， 在获取数据的时候，从队列中获取到ErrorEntity.Id , 这样取出来的Id确保顺序不会变， 再根据这个Id， 通过get取出实体。
        /// 3. 这种方法其实还有待考虑， 虽然List不支持复杂Value， 但是通过json序列化，我们还是可以直接将ErrorEntity放入到List中
        /// </summary>
        /// <param name="errorEntity"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromBody]ErrorEntityDto errorEntity)
        {
            if (errorEntity != null && !string.IsNullOrWhiteSpace(errorEntity.Id))
            {
                if (RedisHelper.EnqueueItemOnList("ErrorEntityQueue", errorEntity.Id))    //先将ErrorEntity.Id放入到队列中，确保顺序不会变
                {
                    if (RedisHelper.Set(errorEntity.Id, errorEntity))  //将实体添加到Redis中
                    {
                        return ReturnPlainText("ok");
                    }
                    return ReturnPlainText("set  error");
                }
                return ReturnPlainText("add item to list error");
            }
            return ReturnPlainText("modle error");
        }

        public HttpResponseMessage ReturnPlainText(string text)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(text, Encoding.UTF8, "text/plain");
            return resp;
        }
    }
}