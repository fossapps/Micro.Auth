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

        public static FieldType RequirePermission(this FieldType type, string permission)
        {
            return type.ApplyDirective(RequirePermissionDirective.DirectiveName, "permission", permission);
        }

        public static FieldBuilder<TSourceType, TReturnType> RequirePermission<TSourceType, TReturnType>(
            this FieldBuilder<TSourceType, TReturnType> type, string permission)
        {
            return type.Directive(RequirePermissionDirective.DirectiveName, "permission", permission);
        }

        public static string? GetAppliedPermission(this FieldType fieldType)
        {
            var applied = fieldType.FindAppliedDirective(RequirePermissionDirective.DirectiveName);
            if (applied == null)
            {
                return null;
            }
            var arg = applied.FindArgument("permission");
            if (arg.Name == "permission")
            {
                return (string) arg.Value;
            }

            return null;
        }
    }
}
