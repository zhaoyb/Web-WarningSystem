using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace warning
{
   public  class SendErrorEntity
    {
        public static void SendError(ClientErrorEntity errorMessage)
        {
            try
            {
                Func<string, string, ContentType, int, Encoding, Encoding, string> sendpostFunc =
                    Common.SendPostRequest;
                string jsonstr = ConvertJsonHelp.ToJson(errorMessage);
                //异步， 确保不会影响主逻辑
                IAsyncResult asyncResult = sendpostFunc.BeginInvoke("http://127.0.0.1:9090/api/Error", jsonstr, ContentType.Json, 3000,
                    Encoding.UTF8,
                    Encoding.UTF8, null, jsonstr);

                string sendResult = sendpostFunc.EndInvoke(asyncResult);

                if (sendResult.ToLower() != "ok")
                {
                    Common.Log("错误信息上传失败，返回信息：" + sendResult);
                }
            }
            catch (Exception exception)
            {
                Common.Log("错误信息上传失败:" + exception.Message);
            }
        }
    }
}
