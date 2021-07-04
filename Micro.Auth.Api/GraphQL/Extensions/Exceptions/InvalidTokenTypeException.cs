using System;

namespace Micro.Auth.Api.GraphQL.Extensions.Exceptions
{
    public class InvalidTokenTypeException : Exception
    {
        public InvalidTokenTypeException(string expectedType) : base($"{expectedType} token not found")
        {
        }
    }
}
