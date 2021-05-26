using GraphQL;
using GraphQL.Types;
using Micro.Auth.Api.GraphQL.Inputs;
using Micro.Auth.Api.GraphQL.Types;
using Micro.Auth.Business.Users;
using Micro.Auth.Business.Users.ViewModels;

namespace Micro.Auth.Api.GraphQL
{
    public class Mutation : ObjectGraphType
    {
        public Mutation(IUserService userService)
        {
            FieldAsync<NonNullGraphType<UserType>, User>("register",
                arguments: new QueryArguments(RegisterInputType.BuildArgument()),
                resolve: x => userService.Create(x.GetArgument<RegisterInput>("input")));

            FieldAsync<NonNullGraphType<UserType>, User>("verifyEmail",
                arguments: new QueryArguments(VerifyEmailInputType.BuildArgument()),
                resolve: x => userService.ConfirmEmail(x.GetArgument<VerifyEmailInput>("input")));
        }
    }
}
