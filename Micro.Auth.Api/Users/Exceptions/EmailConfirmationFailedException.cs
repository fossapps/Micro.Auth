using System;

namespace Micro.Auth.Api.Users.Exceptions
{
    public class EmailConfirmationFailedException : Exception
    {
        public EmailConfirmationFailedException(string message): base(message)
        {

        }
    }
}
