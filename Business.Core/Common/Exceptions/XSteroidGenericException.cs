using System;

namespace Business.Core.Common.Exceptions
{
    public class BusinessGenericException : Exception
    {
        public string ErrorCode { get; set; }

        public BusinessGenericException(string message) : base(message)
        { }

        public BusinessGenericException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
