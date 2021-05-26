using GraphQL;
using GraphQL.Types;
using Micro.Auth.Api.GraphQL.Inputs;
using Micro.Auth.Api.GraphQL.Types;
using Micro.Auth.Business.Users;

namespace Micro.Auth.Api.GraphQL
{
    public class Mutation : ObjectGraphType
    {
        public Mutation(IUserService userService)
        {
            Field<UserType>("register",
                arguments: new QueryArguments(RegisterInputType.BuildArgument()),
                resolve: x => userService.Create(x.GetArgument<RegisterInput>("RegisterInput")));
        }
    }
}
