using System;

namespace Micro.Auth.Business.Users
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("User not found")
        {
        }
    }
}
