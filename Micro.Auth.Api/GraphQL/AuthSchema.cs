using System;
using GraphQL.Types;

namespace Micro.Auth.Api.GraphQL
{
    public class AuthSchema : Schema
    {
        public AuthSchema(IServiceProvider services, Query query, Mutation mutation) : base(services)
        {
            Query = query;
            Mutation = mutation;
        }
    }
}
