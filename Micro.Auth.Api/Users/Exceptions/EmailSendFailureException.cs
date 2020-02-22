using System;

namespace Micro.Auth.Api.Users.Exceptions
{
    public class SendingEmailFailedException : Exception
    {
        public SendingEmailFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
