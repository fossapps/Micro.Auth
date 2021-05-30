using System;

namespace Micro.Auth.Api.GraphQL.Directives.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException() : base("This operation requires logging in")
        {
        }
    }
}
