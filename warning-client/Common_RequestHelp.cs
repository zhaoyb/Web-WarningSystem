using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;

namespace warning
{
    internal partial class Common
    {
      

        /// <summary>
        ///     通讯函数  Post
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="paras"></param>
        /// <param name="cookieId"></param>
        /// <param name="timeout"></param>
        /// <param name="reqEncoding"></param>
        /// <param name="respEncoding"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        internal static string SendPostRequest(string url, string paras, ContentType contentType, int timeout,
            Encoding reqEncoding,
            Encoding respEncoding)
        {
            if (String.IsNullOrEmpty(url))
            {
                return String.Empty;
            }
            if (paras == null)
            {
                paras = String.Empty;
            }
            byte[] data = reqEncoding.GetBytes(paras);
            Stream requestStream = null;
            try
            {
                //Request init
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = timeout;
                request.ContentType = contentType == ContentType.Json ? "application/json" : "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                // Send the data.  
                if (!String.IsNullOrEmpty(paras) && data.Length > 0)
                {
                    requestStream = request.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Close();
                }
                //Response
                var myResponse = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(myResponse.GetResponseStream(), respEncoding);
                string content = reader.ReadToEnd();

                //Dispose
                if (requestStream != null)
                {
                    requestStream.Dispose();
                }
                request.Abort();
                return content;
            }
            catch (Exception ex)
            {
                throw new Exception("发送web post请求出现异常", ex);
            }
        }
    }

    internal enum ContentType
    {
        Xml,
        Json
    }
}