using System;
using System.Globalization;

namespace WebApi.Helpers
{
    // custom exception class for throwing application specific exceptions 
    // that can be caught and handled within the application
    public class NotFoundException : Exception
    {
        public NotFoundException() : base() {}

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, params object[] args) 
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}