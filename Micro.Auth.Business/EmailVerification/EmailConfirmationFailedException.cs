using System;

namespace Micro.Auth.Business.EmailVerification
{
    public class EmailConfirmationFailedException : Exception
    {
        public EmailConfirmationFailedException(string message): base(message)
        {

        }
    }
}
