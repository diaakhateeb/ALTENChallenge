using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;

namespace Logging.Handlers
{
    /// <summary>
    /// Logging different system events.
    /// </summary>
    public class CustomLogger : ILogger
    {
        private readonly string _path;
        /// <summary>
        /// Creates CustomLogger instance.
        /// </summary>
        /// <param name="path">File path where events are getting registered.</param>
        public CustomLogger(string path = @"alten-vehicles-monitor.txt")
        {
            _path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + path;
        }
        /// <summary>
        /// Logs event.
        /// </summary>
        /// <typeparam name="TState">TState value.</typeparam>
        /// <param name="logLevel">Log level.</param>
        /// <param name="eventId">Event Id</param>
        /// <param name="state">State.</param>
        /// <param name="exception">Exception object.</param>
        /// <param name="formatter">Log data Format.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = string.Format("{0}: {1} - {2}", logLevel.ToString(), eventId.Id,
                formatter(state, exception));
            WriteTextToFile(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        private void WriteTextToFile(string message)
        {
            //const string filePath = "c:\\IDGLog.txt";
            using (var streamWriter = new StreamWriter(_path, true))
            {
                streamWriter.WriteLine(message);
                streamWriter.Close();
            }
        }
    }
}
