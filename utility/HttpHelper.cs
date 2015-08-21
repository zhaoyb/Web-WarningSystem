using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Web;

namespace utility
{
    public class HttpHelper
    {
        /// <summary>
        ///     通讯函数
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="para">请求参数</param>
        /// <returns></returns>
        public static string SendPostRequest(string url, string paras, int timeout, Encoding reqEncoding,
            Encoding respEncoding, string contentType = null)
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
                request.ContentType = String.IsNullOrEmpty(contentType)
                    ? "application/x-www-form-urlencoded;"
                    : contentType;
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

                request.Abort();
                return content;
            }
            catch (Exception ex)
            {
                throw new Exception("发送web post请求出现异常", ex);
            }
        }

        /// <summary>
        ///     通讯函数
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="paras">请求参数</param>
        /// <param name="timeout"></param>
        /// <param name="reqEncoding"></param>
        /// <param name="respEncoding"></param>
        /// <param name="contentType"></param>
        /// <param name="allowWriteStreamBuffering"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static string SendPostRequest(string url, string paras, int timeout, Encoding reqEncoding,
            Encoding respEncoding, string contentType = null, bool allowWriteStreamBuffering = true,
            WebProxy proxy = null)
        {
            if (String.IsNullOrEmpty(url))
            {
                return String.Empty;
            }
            byte[] data = reqEncoding.GetBytes(paras);
            Stream requestStream = null;
            try
            {
                //Request init
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = timeout;
                request.ContentType = String.IsNullOrEmpty(contentType)
                    ? "application/x-www-form-urlencoded;"
                    : contentType;
                request.ContentLength = data.Length;
                request.AllowWriteStreamBuffering = allowWriteStreamBuffering;
                request.Proxy = proxy;

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

                request.Abort();
                return content;
            }
            catch (Exception ex)
            {
                throw new Exception("发送web post请求出现异常", ex);
            }
        }

        /// <summary>
        ///     通讯函数
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="paras"></param>
        /// <param name="host"></param>
        /// <param name="timeout"></param>
        /// <param name="encode"></param>
        /// <param name="callBack"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string SendGetRequest(string url, string paras, string host, int timeout, Encoding encode,
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
                if (!string.IsNullOrEmpty(host))
                    request.Host = host;

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
                reader = new StreamReader(stream, encode);
                var strBuilder = new StringBuilder();
                while (-1 != reader.Peek())
                {
                    strBuilder.Append(reader.ReadLine());
                }
                return strBuilder.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (stream != null)
                {
                    stream.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        public static int GetHttpStatus(string url, string paras, string host, int timeout, Encoding encode,
           RemoteCertificateValidationCallback callBack = null, string userAgent = null)
        {
            if (String.IsNullOrEmpty(url))
            {
                return 0;
            }
            if (!url.Contains("?") && !String.IsNullOrEmpty(paras))
            {
                url = url + "?";
            }
            string requestUrl = url + paras;

            HttpWebResponse response = null;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Timeout = timeout;
                request.Method = "GET";
                request.Host = host;

                if (userAgent != null)
                {
                    request.UserAgent = userAgent;
                }
                if (callBack != null)
                {
                    ServicePointManager.ServerCertificateValidationCallback = callBack;
                }
                response = (HttpWebResponse)request.GetResponse();

                return Convert.ToInt32(response.StatusCode.ToString());

            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }
    }
}