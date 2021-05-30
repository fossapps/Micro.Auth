using GraphQL.Types;

namespace Micro.Auth.Api.GraphQL.Inputs
{
    public class ChangePasswordInput : InputObjectGraphType
    {
        public static QueryArgument BuildArgument()
        {
            return new QueryArgument<NonNullGraphType<ChangePasswordInput>> {Name = "input"};
        }

        public ChangePasswordInput()
        {
            Name = "ChangePasswordInput";
            Field<NonNullGraphType<StringGraphType>>("old_password");
            Field<NonNullGraphType<StringGraphType>>("new_password");
        }
    }
}
