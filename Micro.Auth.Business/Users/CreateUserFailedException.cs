using System;

namespace Micro.Auth.Business.Users
{
    public class CreateUserFailedException : Exception
    {
        public CreateUserFailedException(string message) : base(message)
        {
        }
    }
}
