using System;

namespace Micro.Auth.Business.Sessions.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string message) : base(message)
        {
        }
    }
}
