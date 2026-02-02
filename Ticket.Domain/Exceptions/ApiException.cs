using System;

namespace Ticket.Domain.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public ApiException(int statusCode, string message, string? errorCode) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public int StatusCode { get; }
        public string? ErrorCode { get; }
    }
}
