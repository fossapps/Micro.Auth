using System;

namespace Micro.Auth.Business.Users.Exceptions
{
    public class UserAlreadyActivatedException : Exception
    {
        public UserAlreadyActivatedException() : base("Email already verified")
        {
        }
    }
}
