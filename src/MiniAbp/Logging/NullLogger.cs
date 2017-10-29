using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Logging
{
    public class NullLogger : ILogger
    {
        public static ILogger Instance => new NullLogger(); 
        public bool IsDebugEnabled { get; set; }

        public bool IsErrorEnabled { get; set; }

        public bool IsFatalEnabled { get; set; }

        public bool IsInfoEnabled { get; set; }

        public bool IsWarnEnabled { get; set; }



        public ILogger CreateChildLogger(string loggerName)
        {
            return null;
        }

        public void Debug(string message)
        {
        }

        public void Debug(string message, Exception exception)
        {
           
        }

        public void DebugFormat(string format, params object[] args)
        {
           
        }

        public void DebugFormat(Exception exception, string format, params object[] args)
        {
           
        }

        public void Error(string message)
        {
           
        }

        public void Error(string message, Exception exception)
        {
           
        }

        public void ErrorFormat(string format, params object[] args)
        {
           
        }

        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
           
        }

        public void Fatal(string message)
        {
           
        }

        public void Fatal(string message, Exception exception)
        {
           
        }

        public void FatalFormat(string format, params object[] args)
        {
           
        }

        public void FatalFormat(Exception exception, string format, params object[] args)
        {
           
        }

        public void Info(string message)
        {
           
        }

        public void Info(string message, Exception exception)
        {
           
        }

        public void InfoFormat(string format, params object[] args)
        {
           
        }

        public void InfoFormat(Exception exception, string format, params object[] args)
        {
           
        }

        public void Warn(string message)
        {
           
        }

        public void Warn(string message, Exception exception)
        {
           
        }

        public void WarnFormat(string format, params object[] args)
        {
           
        }

        public void WarnFormat(Exception exception, string format, params object[] args)
        {
           
        }
    }
}
