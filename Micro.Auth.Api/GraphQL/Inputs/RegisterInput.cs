using GraphQL.Types;

namespace Micro.Auth.Api.GraphQL.Inputs
{
    public class RegisterInputType : InputObjectGraphType
    {
        public static QueryArgument BuildArgument()
        {
            return new QueryArgument<NonNullGraphType<RegisterInputType>> {Name = "input"};
        }

        public RegisterInputType()
        {
            Name = "RegisterInput";
            Field<NonNullGraphType<StringGraphType>>("username");
            Field<NonNullGraphType<StringGraphType>>("email");
            Field<NonNullGraphType<StringGraphType>>("password");
        }
    }
}
