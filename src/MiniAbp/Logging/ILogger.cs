using System;

namespace MiniAbp.Logging
{
    public interface ILogger
    {
        bool IsDebugEnabled { get; }

        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        ILogger CreateChildLogger(string loggerName);
        void Debug(string message);

        void Debug(string message, Exception exception);
        void DebugFormat(string format, params object[] args);

        void DebugFormat(Exception exception, string format, params object[] args);



        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="message">The message to log</param>
        void Error(string message);

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="message">The message to log</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        void ErrorFormat(string format, params object[] args);

        /// <summary>
        /// Logs an error message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        void ErrorFormat(Exception exception, string format, params object[] args);

      
        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="message">The message to log</param>
        void Fatal(string message);


        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="message">The message to log</param>
        void Fatal(string message, Exception exception);

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        void FatalFormat(string format, params object[] args);

        /// <summary>
        /// Logs a fatal message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        void FatalFormat(Exception exception, string format, params object[] args);


        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="message">The message to log</param>
        void Info(string message);

       
        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="message">The message to log</param>
        void Info(string message, Exception exception);

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        void InfoFormat(string format, params object[] args);

        /// <summary>
        /// Logs an info message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        void InfoFormat(Exception exception, string format, params object[] args);

         /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="message">The message to log</param>
        void Warn(string message); 

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="message">The message to log</param>
        void Warn(string message, Exception exception);

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        void WarnFormat(string format, params object[] args);

        /// <summary>
        /// Logs a warn message.
        /// 
        /// </summary>
        /// <param name="exception">The exception to log</param><param name="format">Format string for the message to log</param><param name="args">Format arguments for the message to log</param>
        void WarnFormat(Exception exception, string format, params object[] args);

    }

}
