using System;
using System.Globalization;

namespace WebApi.Helpers
{
    // custom exception class for throwing application specific exceptions 
    // that can be caught and handled within the application
    public class DefaultException : Exception
    {
        public DefaultException() : base() {}

        public DefaultException(string message) : base(message) { }

        public DefaultException(string message, params object[] args) 
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}