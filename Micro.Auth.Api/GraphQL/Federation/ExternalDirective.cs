using GraphQL.Types;

namespace Micro.Auth.Api.GraphQL.Federation
{
    public class ExternalDirective : DirectiveGraphType
    {

        public const string DirectiveName = "external";
        public override bool? Introspectable => true;

        public ExternalDirective() : base(DirectiveName, DirectiveLocation.FieldDefinition)
        {
        }
    }
}
