using GraphQL.Types;

namespace Micro.Auth.Api.GraphQL.Inputs
{
    public class ResetPasswordInput : InputObjectGraphType
    {
        public static QueryArgument BuildArgument()
        {
            return new QueryArgument<NonNullGraphType<ResetPasswordInput>> {Name = "input"};
        }

        public ResetPasswordInput()
        {
            Name = "ResetPasswordInput";
            Field<NonNullGraphType<StringGraphType>>("login");
            Field<NonNullGraphType<StringGraphType>>("token");
            Field<NonNullGraphType<StringGraphType>>("new_password");
        }
    }
}
