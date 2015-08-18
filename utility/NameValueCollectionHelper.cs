using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utility
{
    public class NameValueCollectionHelper
    {
        public static string PrintNameValueCollection(NameValueCollection nameValueCollection)
        {
            StringBuilder namevalueText = new StringBuilder();
            foreach (string key in nameValueCollection.AllKeys)
            {
                namevalueText.AppendLine(key + ":" + nameValueCollection[key]);
            }
            return namevalueText.ToString();
        }
    }
}
