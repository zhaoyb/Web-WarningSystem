using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

namespace warning
{
    public class ErrorLogModule : IHttpModule
    {

        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.Error += context_Error;
        }

        #endregion

        void context_Error(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            ErrorEntity errorMessage = new ErrorEntity(application.Server.GetLastError().GetBaseException(), application.Context);
            SendError(errorMessage);
        }

        private static void SendError(ErrorEntity errorMessage)
        {
            try
            {
                Func<string, string, ContentType, int, Encoding, Encoding, string> sendpostFunc =
                    Common.SendPostRequest;
                string jsonstr = ConvertJsonHelp.ToJson(errorMessage);
                //异步， 确保不会影响主逻辑
                IAsyncResult asyncResult = sendpostFunc.BeginInvoke("http://demo.lottery.com/api/Error", jsonstr, ContentType.Json, 3000,
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
