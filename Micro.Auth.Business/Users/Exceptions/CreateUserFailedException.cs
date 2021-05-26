using System;

namespace Micro.Auth.Business.Users.Exceptions
{
    public class CreateUserFailedException : Exception
    {
        public CreateUserFailedException(string message) : base(message)
        {
        }
    }
}
