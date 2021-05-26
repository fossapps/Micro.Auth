using GraphQL.Types;

namespace Micro.Auth.Api.GraphQL.Inputs
{
    public class VerifyEmailInputType : InputObjectGraphType
    {
        public static QueryArgument BuildArgument()
        {
            return new QueryArgument<NonNullGraphType<VerifyEmailInputType>> {Name = "input"};
        }

        public VerifyEmailInputType()
        {
            Name = "VerifyEmailInput";
            Field<NonNullGraphType<StringGraphType>>("login");
            Field<NonNullGraphType<StringGraphType>>("token");
        }
    }
}
