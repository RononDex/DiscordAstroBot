using log4net;
using log4net.Core;
using System;

namespace DiscordAstroBot
{
    /// <summary>
    /// Generic helper class for log4net
    /// </summary>
    public static class Log<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(T));

        /// <summary>
        /// <see cref="ILogger"/>
        /// </summary>
        /// <returns></returns>
        public static ILogger GetLogger()
        {
            return Logger.Logger;
        }

        /// <summary>
        /// <see cref="ILog.Debug(object)"/>
        /// </summary>
        public static void Debug(object message)
        {
            Logger.Debug(message);
        }

        /// <summary>
        /// <see cref="ILog.Debug(object)"/>
        /// </summary>
        public static void Debug(object message, Exception exception)
        {
            Logger.Debug(message, exception);
        }

        /// <summary>
        /// <see cref="ILog.DebugFormat(string,object[])"/>
        /// </summary>
        public static void DebugFormat(string format, params object[] args)
        {
            Logger.DebugFormat(format, args);
        }

        /// <summary>
        /// <see cref="ILog.DebugFormat(string,object[])" />
        /// </summary>
        public static void DebugFormat(string format, object arg0)
        {
            Logger.DebugFormat(format, arg0);
        }

        /// <summary>
        /// <see cref="ILog.DebugFormat(string,object[])" />
        /// </summary>
        public static void DebugFormat(string format, object arg0, object arg1)
        {
            Logger.DebugFormat(format, arg0, arg1);
        }

        /// <summary>
        /// <see cref="ILog.DebugFormat(string,object[])" />
        /// </summary>
        public static void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            Logger.DebugFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// <see cref="ILog.DebugFormat(string,object[])" />
        /// </summary>
        public static void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.DebugFormat(provider, format, args);
        }

        /// <summary>
        /// <see cref="ILog.Info(object)" />
        /// </summary>
        public static void Info(object message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// <see cref="ILog.Info(object)" />
        /// </summary>
        public static void Info(object message, Exception exception)
        {
            Logger.Info(message, exception);
        }

        /// <summary>
        /// <see cref="ILog.InfoFormat(string,object[])" />
        /// </summary>
        public static void InfoFormat(string format, params object[] args)
        {
            Logger.InfoFormat(format, args);
        }

        /// <summary>
        /// <see cref="ILog.InfoFormat(string,object[])" />
        /// </summary>
        public static void InfoFormat(string format, object arg0)
        {
            Logger.InfoFormat(format, arg0);
        }

        /// <summary>
        /// <see cref="ILog.InfoFormat(string,object[])" />
        /// </summary>
        public static void InfoFormat(string format, object arg0, object arg1)
        {
            Logger.InfoFormat(format, arg0, arg1);
        }

        /// <summary>
        /// <see cref="ILog.InfoFormat(string,object[])" />
        /// </summary>
        public static void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            Logger.InfoFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// <see cref="ILog.InfoFormat(string,object[])" />
        /// </summary>
        public static void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.InfoFormat(provider, format, args);
        }

        /// <summary>
        /// <see cref="ILog.Warn(object)" />
        /// </summary>
        public static void Warn(object message)
        {
            Logger.Warn(message);
        }

        /// <summary>
        /// <see cref="ILog.Warn(object)" />
        /// </summary>
        public static void Warn(object message, Exception exception)
        {
            Logger.Warn(message, exception);
        }

        /// <summary>
        /// <see cref="ILog.WarnFormat(string,object[])" />
        /// </summary>
        public static void WarnFormat(string format, params object[] args)
        {
            Logger.WarnFormat(format, args);
        }

        /// <summary>
        /// <see cref="ILog.WarnFormat(string,object[])" />
        /// </summary>
        public static void WarnFormat(string format, object arg0)
        {
            Logger.WarnFormat(format, arg0);
        }

        /// <summary>
        /// <see cref="ILog.WarnFormat(string,object[])" />
        /// </summary>
        public static void WarnFormat(string format, object arg0, object arg1)
        {
            Logger.WarnFormat(format, arg0, arg1);
        }

        /// <summary>
        /// <see cref="ILog.WarnFormat(string,object[])" />
        /// </summary>
        public static void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            Logger.WarnFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// <see cref="ILog.WarnFormat(string,object[])" />
        /// </summary>
        public static void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.WarnFormat(provider, format, args);
        }

        /// <summary>
        /// <see cref="ILog.Error(object)" />
        /// </summary>
        public static void Error(object message)
        {
            Logger.Error(message);
        }

        /// <summary>
        /// <see cref="ILog.Error(object)" />
        /// </summary>
        public static void Error(object message, Exception exception)
        {
            Logger.Error(message, exception);
        }

        /// <summary>
        /// <see cref="ILog.ErrorFormat(string,object[])" />
        /// </summary>
        public static void ErrorFormat(string format, params object[] args)
        {
            Logger.ErrorFormat(format, args);
        }

        /// <summary>
        /// <see cref="ILog.ErrorFormat(string,object[])" />
        /// </summary>
        public static void ErrorFormat(string format, object arg0)
        {
            Logger.ErrorFormat(format, arg0);
        }

        /// <summary>
        /// <see cref="ILog.ErrorFormat(string,object[])" />
        /// </summary>
        public static void ErrorFormat(string format, object arg0, object arg1)
        {
            Logger.ErrorFormat(format, arg0, arg1);
        }

        /// <summary>
        /// <see cref="ILog.ErrorFormat(string,object[])" />
        /// </summary>
        public static void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            Logger.ErrorFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// <see cref="ILog.ErrorFormat(string,object[])" />
        /// </summary>
        public static void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.ErrorFormat(provider, format, args);
        }

        /// <summary>
        /// <see cref="ILog.Fatal(object)" />
        /// </summary>
        public static void Fatal(object message)
        {
            Logger.Fatal(message);
        }

        /// <summary>
        /// <see cref="ILog.Fatal(object)" />
        /// </summary>
        public static void Fatal(object message, Exception exception)
        {
            Logger.Fatal(message, exception);
        }

        /// <summary>
        /// <see cref="ILog.FatalFormat(string,object[])" />
        /// </summary>
        public static void FatalFormat(string format, params object[] args)
        {
            Logger.FatalFormat(format, args);
        }

        /// <summary>
        /// <see cref="ILog.FatalFormat(string,object[])" />
        /// </summary>
        public static void FatalFormat(string format, object arg0)
        {
            Logger.FatalFormat(format, arg0);
        }

        /// <summary>
        /// <see cref="ILog.FatalFormat(string,object[])" />
        /// </summary>
        public static void FatalFormat(string format, object arg0, object arg1)
        {
            Logger.FatalFormat(format, arg0, arg1);
        }

        /// <summary>
        /// <see cref="ILog.FatalFormat(string,object[])" />
        /// </summary>
        public static void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            Logger.FatalFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// <see cref="ILog.FatalFormat(string,object[])" />
        /// </summary>
        public static void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.FatalFormat(provider, format, args);
        }

        /// <summary>
        /// <see cref="ILog.IsDebugEnabled"/>
        /// </summary>
        public static bool IsDebugEnabled
        {
            get { return Logger.IsDebugEnabled; }
        }

        /// <summary>
        /// <see cref="ILog.IsInfoEnabled"/>
        /// </summary>
        public static bool IsInfoEnabled
        {
            get { return Logger.IsInfoEnabled; }
        }

        /// <summary>
        /// <see cref="ILog.IsWarnEnabled"/>
        /// </summary>
        public static bool IsWarnEnabled
        {
            get { return Logger.IsWarnEnabled; }
        }

        /// <summary>
        /// <see cref="ILog.IsErrorEnabled"/>
        /// </summary>
        public static bool IsErrorEnabled
        {
            get { return Logger.IsErrorEnabled; }
        }

        /// <summary>
        /// <see cref="ILog.IsFatalEnabled"/>
        /// </summary>
        public static bool IsFatalEnabled
        {
            get { return Logger.IsFatalEnabled; }
        }
    }
}
