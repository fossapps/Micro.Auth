using GraphQL.Types;

namespace Micro.Auth.Api.GraphQL.Inputs
{
    public class VerifyEmailInputType : InputObjectGraphType
    {
        public static QueryArgument BuildArgument()
        {
            return new QueryArgument<NonNullGraphType<VerifyEmailInputType>> {Name = "RegisterInput"};
        }

        public VerifyEmailInputType()
        {
            Field<NonNullGraphType<StringGraphType>>("login");
            Field<NonNullGraphType<StringGraphType>>("token");
        }
    }
}
