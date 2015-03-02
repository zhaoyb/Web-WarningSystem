using System;
using System.Configuration;

namespace utility
{
    /// <summary>
    /// web.config操作类
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// 得到AppSettings中的配置字符串信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigString(string key)
        {
            string value = "";
            try
            {
                value = ConfigurationManager.AppSettings[key];
            }
            catch (Exception exception)
            {
                LogHelper.Error("获取" + key + "配置失败" + exception);
            }
            return value;
        }

        /// <summary>
        /// 得到AppSettings中的配置Bool信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetConfigBool(string key)
        {
            bool result = false;
            string cfgVal = GetConfigString(key);
            if (!string.IsNullOrEmpty(cfgVal))
            {
                try
                {
                    result = bool.Parse(cfgVal);
                }
                catch (FormatException exception)
                {
                    LogHelper.Error("获取" + key + "配置失败" + exception);
                }
            }
            return result;
        }
        /// <summary>
        /// 得到AppSettings中的配置Decimal信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static decimal GetConfigDecimal(string key)
        {
            decimal result = 0;
            string cfgVal = GetConfigString(key);
            if (!string.IsNullOrEmpty(cfgVal))
            {
                try
                {
                    result = decimal.Parse(cfgVal);
                }
                catch (FormatException exception)
                {
                    LogHelper.Error("获取" + key + "配置失败" + exception);
                }
            }
            return result;
        }
        /// <summary>
        /// 得到AppSettings中的配置int信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetConfigInt(string key)
        {
            int result = 0;
            string cfgVal = GetConfigString(key);
            if (!string.IsNullOrEmpty(cfgVal))
            {
                try
                {
                    result = int.Parse(cfgVal);
                }
                catch (FormatException exception)
                {
                    LogHelper.Error("获取" + key + "配置失败" + exception);
                }
            }
            return result;
        }
    }
}
