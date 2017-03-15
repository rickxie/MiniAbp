using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MiniAbp.Runtime;

namespace MiniAbp.Logging
{
    public class FileLogger : ILogger
    {
        public bool IsDebugEnabled => true;
        public bool IsErrorEnabled => true;
        public bool IsFatalEnabled => true;
        public bool IsInfoEnabled => true;
        public bool IsWarnEnabled => true;
        public object objLock = new object();

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

        /// <summary>
        /// 保存log
        /// </summary>
        /// <param name="content"></param>
        private void SaveLog(string content)
        {
            var logDir = AppPath.GetRelativeDir("Logs/");
            var filePath = logDir + "logs.txt";
            try
            {
                lock (objLock)
                {
                    using (var sr = new StreamWriter(filePath, true, Encoding.Default))
                    {
                        sr.WriteLine(content);
                        sr.Close();
                    }
                }
            }
            catch (Exception)
            {
            }

            var info = new FileInfo(filePath);
            var mbSize = info.Length/(1024*1024);
            if (mbSize >= 2)
            {
                var files = Directory.GetFiles(logDir);
                var rex = new Regex("logs([0-9]+).txt");
                var maxCount = 0;
                foreach (var file in files)
                {
                    var numGroup = rex.Match(file);
                    if (numGroup.Groups.Count == 2)
                    {
                        var count = Convert.ToInt32(numGroup.Groups[1].Value);
                        if (count > maxCount)
                        {
                            maxCount = count;
                        }
                    }
                }
                var newPath = logDir + string.Format("logs{0}.txt", ++maxCount);
                info.MoveTo(newPath);
            }

        }


        private void BuildLogAndSave(LoggerLevel level, Exception ex = null, string message = null)
        {
            var sb = new StringBuilder();
            var curDate = DateTime.Now;
            sb.Append(level);
            sb.Append(" ");
            sb.Append(curDate.ToString(CultureInfo.InvariantCulture));
            sb.Append(" ");
            sb.Append(curDate.Millisecond.ToString());
            sb.Append(" ");
            if (message != null)
            {
                sb.AppendLine(message);
            }
            if (ex != null)
            {
                sb.AppendLine(ex.ToString());
            }
            sb.Append(Environment.NewLine);
            SaveLog(sb.ToString());
        }
    }
}
