using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace warning
{
    internal partial class Common
    {
        internal static Dictionary<string,string> ConvertCollectionToDictionary(NameValueCollection collection)
        {
            var dic = new Dictionary<string, string>();

            if (collection == null || collection.Count == 0)
                return dic;

            foreach (string key in collection)
            {
                dic.Add(key, collection[key]);
            }
            return dic;
        }

        internal static Dictionary<string, string> ConvertCollectionToDictionary(HttpCookieCollection cookies)
        {
            var dic = new Dictionary<string, string>();
            if (cookies == null || cookies.Count == 0)
                return dic;


            foreach (HttpCookie httpCookie in cookies)
            {
                dic.Add(httpCookie.Name, httpCookie.Value);
            }
            return dic;
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

        internal static void Log(string logs)
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
