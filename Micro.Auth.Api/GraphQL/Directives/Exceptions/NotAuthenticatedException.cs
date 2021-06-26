using System;

namespace Micro.Auth.Api.GraphQL.Directives.Exceptions
{
    public class NotAuthenticatedException : Exception
    {
        public NotAuthenticatedException() : base("This operation requires logging in")
        {
        }
    }
}
