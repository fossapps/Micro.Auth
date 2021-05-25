using System;

namespace Micro.Auth.Business.Users.Exceptions
{
    public class EmailConfirmationFailedException : Exception
    {
        public EmailConfirmationFailedException(string message): base(message)
        {

        }
    }
}
