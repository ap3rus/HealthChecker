using System;

namespace HealthChecker.ConsoleApp
{
    public class ConfigurationValidationException : Exception
    {
        public ConfigurationValidationException()
        {
        }

        public ConfigurationValidationException(string message)
            : base(message)
        {
        }

        public ConfigurationValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}