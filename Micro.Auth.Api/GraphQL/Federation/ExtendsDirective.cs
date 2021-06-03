using GraphQL.Types;

namespace Micro.Auth.Api.GraphQL.Federation
{
    public class ExtendsDirective : DirectiveGraphType
    {
        public const string DirectiveName = "external";
        public override bool? Introspectable => true;

        public ExtendsDirective() : base(DirectiveName, DirectiveLocation.Object, DirectiveLocation.Interface)
        {
        }
    }
}
