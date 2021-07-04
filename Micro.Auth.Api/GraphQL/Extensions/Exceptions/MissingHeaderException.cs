using System;

namespace Micro.Auth.Api.GraphQL.Extensions.Exceptions
{
    public class MissingHeaderException : Exception
    {
        public MissingHeaderException(string headerName) : base($"Missing {headerName} header")
        {
        }
    }
}
