using GraphQL.Types;

namespace Micro.Auth.Api.GraphQL.Federation
{
    public class KeyDirective : DirectiveGraphType
    {
        public const string DirectiveName = "key";
        public override bool? Introspectable => true;

        public KeyDirective() : base(DirectiveName, DirectiveLocation.Object, DirectiveLocation.Interface)
        {
            Repeatable = true;
            Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = "fields",
                Description = "_FieldSet"
            });
        }
    }
}
