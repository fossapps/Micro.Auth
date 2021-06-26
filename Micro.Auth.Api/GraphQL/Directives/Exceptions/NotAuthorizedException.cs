using System;

namespace Micro.Auth.Api.GraphQL.Directives.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException() : base("You do not have sufficient permission to perform this operation")
        {
        }
    }
}
