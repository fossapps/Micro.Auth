using System;

namespace Micro.Auth.Business.Internal.Keys.Exceptions
{
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException(string message) : base(message)
        {
        }
    }
}
