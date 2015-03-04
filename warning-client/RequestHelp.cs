using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;

namespace warning
{
    internal class RequestHelp
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


        /// <summary>
        ///     通讯函数   Get
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="paras"></param>
        /// <param name="timeout"></param>
        /// <param name="encode"></param>
        /// <param name="callBack"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        internal static string SendGetRequest(string url, string paras, int timeout, Encoding encode,
            RemoteCertificateValidationCallback callBack = null, string userAgent = null)
        {
            if (String.IsNullOrEmpty(url))
            {
                return String.Empty;
            }
            if (!url.Contains("?") && !String.IsNullOrEmpty(paras))
            {
                url = url + "?";
            }
            string requestUrl = url + paras;

            HttpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Timeout = timeout;
                request.Method = "GET";
                if (userAgent != null)
                {
                    request.UserAgent = userAgent;
                }
                if (callBack != null)
                {
                    ServicePointManager.ServerCertificateValidationCallback = callBack;
                }

                response = (HttpWebResponse)request.GetResponse();
                stream = response.GetResponseStream();
                if (stream != null) reader = new StreamReader(stream, encode);
                var strBuilder = new StringBuilder();
                while (reader != null && -1 != reader.Peek())
                {
                    strBuilder.Append(reader.ReadLine());
                }
                return strBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("发送web get请求出现异常", ex);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }

    }

    internal enum ContentType
    {
        Xml,
        Json
    }
}