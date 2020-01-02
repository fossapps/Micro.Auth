using System;

namespace Micro.Auth.Api.Authentication
{
    public class BadBasicAuthorizationDataException : Exception
    {
        public BadBasicAuthorizationDataException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
