using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utility
{
    public class DictionaryHelper
    {
        public static string PrintDictionary(Dictionary<string,string> nameValueCollection)
        {
            StringBuilder namevalueText = new StringBuilder();
            foreach (string key in nameValueCollection.Keys)
            {
                namevalueText.AppendLine(key + "=" + nameValueCollection[key]);
            }
            return namevalueText.ToString();
        }
    }
}
