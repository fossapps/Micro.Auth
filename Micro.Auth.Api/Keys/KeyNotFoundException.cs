using System;

namespace Micro.Auth.Api.Keys
{
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException(string message) : base(message)
        {
        }
    }
}
