using System;

namespace Business.Core.Messaging.Common.Exceptions
{
    public class SmsFormatException : Exception
    {
        public SmsFormatException(string message)
            : base(message)
        { }
    }
}
