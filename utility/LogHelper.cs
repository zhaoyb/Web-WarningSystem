using System;
using log4net;

namespace utility
{
    public class LogHelper
    {
        private static readonly ILog Loger = LogManager.GetLogger("LogSystem");
        public static void Debug(object message)
        {
            Loger.Debug(message);
        }

        public static void Debug(object message, Exception exception)
        {
            Loger.Debug(message, exception);
        }

        public static void Info(object message)
        {
            Loger.Info(message);
        }

        public static void Info(object message, Exception exception)
        {
            Loger.Info(message, exception);
        }

        public static void Error(object message)
        {
            Loger.Error(message);
        }

        public static void Error(object message, Exception exception)
        {
            Loger.Error(message, exception);
        }

        public static void Warn(object message)
        {
            Loger.Warn(message);
        }

        public static void Warn(object message, Exception exception)
        {
            Loger.Warn(message, exception);
        }
    }
}
