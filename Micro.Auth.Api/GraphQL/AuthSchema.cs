using System;
using Micro.Auth.Api.GraphQL.Types;
using Micro.GraphQL.Federation;
using Micro.Auth.Api.GraphQL.Directives;

namespace Micro.Auth.Api.GraphQL
{
    public class AuthSchema : Schema<EntityType>
    {
        public AuthSchema(IServiceProvider services, Query query, Mutation mutation) : base(services)
        {
            Query = query;
            Mutation = mutation;
            Directives.Register(new AuthorizeDirective());
            Directives.Register(new RequirePermissionDirective());
            RegisterVisitor(typeof(AuthorizeDirectiveVisitor));
            RegisterVisitor(typeof(RequirePermissionDirectiveVisitor));
        }
    }
}
