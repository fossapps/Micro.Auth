using System;

namespace Micro.Auth.Business.EmailVerification
{
    public class UserAlreadyActivatedException : Exception
    {
        public UserAlreadyActivatedException() : base("Email already verified")
        {
        }
    }
}
