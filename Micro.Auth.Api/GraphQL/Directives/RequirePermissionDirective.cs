using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Utilities;
using Micro.Auth.Api.GraphQL.Directives.Exceptions;
using Micro.Auth.Api.GraphQL.Directives.Extensions;
using Microsoft.AspNetCore.Http;

namespace Micro.Auth.Api.GraphQL.Directives
{
    public class RequirePermissionDirective : DirectiveGraphType
    {
        public const string DirectiveName = "requirePermission";

        public RequirePermissionDirective() : base(
            DirectiveName,
            DirectiveLocation.Field,
            DirectiveLocation.Mutation,
            DirectiveLocation.Query,
            DirectiveLocation.FieldDefinition)
        {
            Description = "Used to check if a user has a set of permission";
            Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = "permission",
                Description = "Permission to check for",
            });
        }
    }
    public class RequirePermissionDirectiveVisitor : BaseSchemaNodeVisitor
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public RequirePermissionDirectiveVisitor(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public override void VisitObjectFieldDefinition(FieldType field, IObjectGraphType type, ISchema schema)
        {
            var permission = field.GetAppliedPermission();
            if (permission == null)
            {
                return;
            }

            var isAuthorized = _contextAccessor
                .HttpContext
                ?.User
                .HasClaim(x => x.Type == "Permission" && x.Value == permission);

            if (isAuthorized == true)
            {
                return;
            }

            field.Resolver = new AsyncFieldResolver<object>(async context => throw new NotAuthorizedException());
        }
    }
}
