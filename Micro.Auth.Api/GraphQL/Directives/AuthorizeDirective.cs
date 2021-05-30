using GraphQL;
using GraphQL.Types;
using GraphQL.Utilities;
using Micro.Auth.Api.GraphQL.Directives.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Micro.Auth.Api.GraphQL.Directives
{
    public class AuthorizeDirective : DirectiveGraphType
    {
        public const string DirectiveName = "authorize";
        public AuthorizeDirective() : base(
            DirectiveName,
            DirectiveLocation.Field,
            DirectiveLocation.Mutation,
            DirectiveLocation.Query,
            DirectiveLocation.FieldDefinition)
        {
        }
    }

    public class AuthorizeDirectiveVisitor : BaseSchemaNodeVisitor
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public AuthorizeDirectiveVisitor(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public override void VisitObjectFieldDefinition(FieldType field, IObjectGraphType type, ISchema schema)
        {
            var applied = field.FindAppliedDirective(AuthorizeDirective.DirectiveName);
            if (applied == null)
            {
                return;
            }

            var isAuthenticated = _contextAccessor.HttpContext?.User.Identity?.IsAuthenticated;
            if (isAuthenticated == true)
            {
                return;
            }

            throw new NotAuthorizedException();
        }
    }
}
