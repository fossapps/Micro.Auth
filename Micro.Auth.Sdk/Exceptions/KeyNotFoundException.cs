using System;

namespace Micro.Auth.Sdk.Exceptions
{
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException(string? message) : base(message)
        {
        }
    }
}
