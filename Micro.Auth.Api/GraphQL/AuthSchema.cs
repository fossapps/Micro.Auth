using System;
using Micro.Auth.Api.GraphQL.Federation;
using Micro.Auth.Api.GraphQL.Directives;

namespace Micro.Auth.Api.GraphQL
{
    public class AuthSchema : Schema
    {
        public AuthSchema(IServiceProvider services, Query query, Mutation mutation) : base(services)
        {
            Query = query;
            Mutation = mutation;
            Directives.Register(new AuthorizeDirective());
            RegisterVisitor(typeof(AuthorizeDirectiveVisitor));
        }
    }
}
