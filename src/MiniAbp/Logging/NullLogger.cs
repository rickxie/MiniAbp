using System;
using System.Globalization;
using System.Text;

namespace MiniAbp.Logging
{
    public class NullLogger : ILogger
    {
        public bool IsDebugEnabled { get; }
        public bool IsErrorEnabled { get; }
        public bool IsFatalEnabled { get; }
        public bool IsInfoEnabled { get; }
        public bool IsWarnEnabled { get; }
        public static  ILogger Instance = new NullLogger();
        public ILogger CreateChildLogger(string loggerName)
        {
            throw new NotImplementedException();
        }

        public void Debug(string message)
        {
            if (IsDebugEnabled)
            BuildLogAndSave(LoggerLevel.Debug, message: message);
        }

        public void Debug(string message, Exception exception)
        {
            if (IsDebugEnabled)
                BuildLogAndSave(LoggerLevel.Debug, exception, message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (IsDebugEnabled)
                BuildLogAndSave(LoggerLevel.Debug, message: string.Format(format,args));
        }

        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            if (IsDebugEnabled)
                BuildLogAndSave(LoggerLevel.Debug,exception, string.Format(format,args));
        }


        public void Error(string message)
        {
            if (IsErrorEnabled)
                BuildLogAndSave(LoggerLevel.Error,  message: message);
        }

        public void Error(string message, Exception exception)
        {
            if (IsErrorEnabled)
                BuildLogAndSave(LoggerLevel.Error, exception, message: message);
        }

        public void ErrorFormat(string format, params object[] args)
        {

            if (IsErrorEnabled)
                BuildLogAndSave(LoggerLevel.Error, message: string.Format(format,args));
        }

        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            if (IsErrorEnabled)
                BuildLogAndSave(LoggerLevel.Error, exception, string.Format(format,args));
        }



        public void Fatal(string message)
        {
            if (IsFatalEnabled)
                BuildLogAndSave(LoggerLevel.Fatal, message: message);
        }

        public void Fatal(string message, Exception exception)
        {
            if (IsFatalEnabled)
                BuildLogAndSave(LoggerLevel.Fatal, exception, message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            if (IsFatalEnabled)
                BuildLogAndSave(LoggerLevel.Fatal, message: string.Format(format, args));

        }

        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            if (IsFatalEnabled)
                BuildLogAndSave(LoggerLevel.Fatal, exception, string.Format(format, args));
        }

        public void Info(string message)
        {
            if (IsInfoEnabled)
                BuildLogAndSave(LoggerLevel.Info, message: message);
        }

        public void Info(string message, Exception exception)
        {
            if (IsInfoEnabled)
            BuildLogAndSave(LoggerLevel.Info, exception, message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (IsInfoEnabled)
                BuildLogAndSave(LoggerLevel.Info, message:string.Format(format,args));
        }

        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            if (IsInfoEnabled)
                BuildLogAndSave(LoggerLevel.Info, exception, string.Format(format,args));
        }

        public void Warn(string message)
        {
            if (IsWarnEnabled)
                BuildLogAndSave(LoggerLevel.Warn, message:message);
        }


        public void Warn(string message, Exception exception)
        {
            if (IsWarnEnabled)
                BuildLogAndSave(LoggerLevel.Warn, exception, message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (IsWarnEnabled)
                BuildLogAndSave(LoggerLevel.Warn, null, string.Format(format, args));
        }

        public void WarnFormat(Exception exception, string format, params object[] args)
        { 
            if (IsWarnEnabled)
                BuildLogAndSave(LoggerLevel.Warn, exception, string.Format(format, args));
        }

       


        private void SaveLog(string content)
        {

        }
         

        private void BuildLogAndSave(LoggerLevel level, Exception ex = null, string message = null)
        {
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            var curDate = DateTime.Now;
            sb.Append(level);
            sb.Append(" ");
            sb.Append(curDate.ToString(CultureInfo.InvariantCulture));
            sb.Append(" ");
            sb.Append(curDate.Millisecond.ToString());
            if (message != null)
            {
                sb.AppendLine(message);
            }
            if (ex != null)
            {
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.StackTrace);
            }
            sb.Append(Environment.NewLine);
            SaveLog(sb.ToString());
        }
    }
}
