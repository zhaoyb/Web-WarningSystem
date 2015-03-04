using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace warning
{
    public class Utility
    {
        internal static NameValueCollection CopyCollection(NameValueCollection collection)
        {
            if (collection == null || collection.Count == 0)
                return new NameValueCollection();
            return new NameValueCollection(collection);
        }

        internal static NameValueCollection CopyCollection(HttpCookieCollection cookies)
        {
            if (cookies == null || cookies.Count == 0)
                return new NameValueCollection();

            NameValueCollection cookieCollection = new NameValueCollection(cookies.Count);
            for (int i = 0; i < cookies.Count; i++)
            {
                HttpCookie cookie = cookies[i];
                cookieCollection.Add(cookie.Name, cookie.Value);
            }

            return cookieCollection;
        }


        internal static string GetLocalIp()
        {
            string localIp = "";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = ip.ToString();
                }
            }
            return localIp;
        }

        public static void SendError(ErrorMessage errorMessage)
        {
            try
            {
                Func<string, string, ContentType, int, Encoding, Encoding, string> sendpostFunc =
                    RequestHelp.SendPostRequest;
                string jsonstr = ConvertJsonHelp.ToJson(errorMessage);
                IAsyncResult asyncResult = sendpostFunc.BeginInvoke("http://demo.lottery.com/api/Error", jsonstr, ContentType.Json, 3000,
                    Encoding.UTF8,
                    Encoding.UTF8, null, jsonstr);

                string sendResult = sendpostFunc.EndInvoke(asyncResult);

                if (sendResult.ToLower() != "ok")
                {
                    Log("错误信息上传失败，返回信息：" + sendResult);
                }
            }
            catch (Exception exception)
            {
                Log("错误信息上传失败:" + exception.Message);
            }
        }

        private static void Log(string logs)
        {
            using (FileStream fsFile = new FileStream(HttpRuntime.AppDomainAppPath + @"\webwarninglog\log.txt", FileMode.Append))
            {
                using (StreamWriter swWriter = new StreamWriter(fsFile))
                {
                    swWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + logs);
                }
            }
        }
    }
}
