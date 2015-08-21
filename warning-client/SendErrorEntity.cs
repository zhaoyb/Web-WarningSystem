using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace warning
{
    public class SendErrorEntity
    {


        delegate string SendPostdelegate(string url, string paras, ContentType contentType, int timeout,
          Encoding reqEncoding,
          Encoding respEncoding);

        private static SendPostdelegate sendPostdelegate = Common.SendPostRequest;

        public static void SendError(ClientErrorEntity errorMessage)
        {
            try
            {
                string jsonstr = JsonConvert.SerializeObject(errorMessage);
                //异步， 确保不会影响主逻辑
                sendPostdelegate.BeginInvoke("http://192.168.16.39:90/api/Error", jsonstr, ContentType.Json, 3000,
                   Encoding.UTF8,
                   Encoding.UTF8, Response, sendPostdelegate);   //这里请求地址 建议改为域名的方式，  如果没有域名，建议请求地址和webtoken类似为可配置
            }
            catch (Exception exception)
            {
                Common.Log("错误信息上传失败:" + exception.Message);
            }
        }

        public static void Response(IAsyncResult asyncResult)
        {
            var postdelegate = asyncResult.AsyncState as SendPostdelegate;
            if (postdelegate != null)
            {
                var sendResult = postdelegate.EndInvoke(asyncResult);

                if (sendResult.ToLower() != "ok")
                {
                    Common.Log("返回错误信息：" + sendResult);
                }
            }
        }
    }
}
