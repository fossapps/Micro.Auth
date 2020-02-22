using System;

namespace Micro.Auth.Api.Authentication.Exceptions
{
    public class BadBasicAuthorizationDataException : Exception
    {
        public BadBasicAuthorizationDataException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
