
using System;

namespace Business.Core.Messaging.Common.Exceptions
{
    public class EmailFormatException : Exception
    {
        public EmailFormatException(string message) : base(message)
        { }
    }
}
