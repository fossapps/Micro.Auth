using System;

namespace Micro.Auth.Api.Keys.Exceptions
{
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException(string message) : base(message)
        {
        }
    }
}
