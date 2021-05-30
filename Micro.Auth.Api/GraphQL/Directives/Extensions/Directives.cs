using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;

namespace Micro.Auth.Api.GraphQL.Directives.Extensions
{
    public static class Directives
    {
        public static FieldType Authorize(this FieldType type)
        {
            return type.ApplyDirective(AuthorizeDirective.DirectiveName);
        }
        public static FieldBuilder<TSourceType, TReturnType> Authorize<TSourceType, TReturnType>(this FieldBuilder<TSourceType, TReturnType> type)
        {
            return type.Directive(AuthorizeDirective.DirectiveName);
        }
    }
}
