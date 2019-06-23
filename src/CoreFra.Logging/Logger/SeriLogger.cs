using System;
using Microsoft.Extensions.Logging;

namespace CoreFra.Logging
{
    public class SeriLogger : ICustomLogger
    {
        private static ILogger<SeriLogger> _logger;

        public SeriLogger(ILogger<SeriLogger> logger)
        {
            _logger = logger;
        }

        public void ErrorException(string message, System.Exception exception)
        {
            GenerateLog<dynamic>(message, LogLevel.Error, exception, null);
        }

        public void Error(string message)
        {
            GenerateLog<dynamic>(message, LogLevel.Error, null, null);
        }

        private static void GenerateLog<T>(string message, LogLevel logLevel, Exception ex, T data)
        {
            if (ex == null)
            {
                ex = new Exception(message);
            }

            _logger.LogError(ex, message);
        }
    }
}