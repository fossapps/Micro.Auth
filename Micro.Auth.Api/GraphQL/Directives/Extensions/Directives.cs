using GraphQL;
using GraphQL.Types;

namespace Micro.Auth.Api.GraphQL.Directives.Extensions
{
    public static class Directives
    {
        public static FieldType Authorize(this FieldType type)
        {
            return type.ApplyDirective(AuthorizeDirective.DirectiveName);
        }
    }
}
