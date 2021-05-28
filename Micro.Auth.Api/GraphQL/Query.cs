using GraphQL;
using GraphQL.Types;
using Micro.Auth.Api.GraphQL.Types;
using Micro.Auth.Business.Availability;
using Micro.Auth.Business.Users;

namespace Micro.Auth.Api.GraphQL
{
    public class Query : ObjectGraphType
    {
        public Query(IUserService userService, IAvailabilityService availabilityService)
        {
            FieldAsync<NonNullGraphType<UserType>, User>("user",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "id"}),
                resolve: x => userService.FindById(x.GetArgument<string>("id")));

            FieldAsync<NonNullGraphType<UserType>, User>("userByLogin",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "login"}),
                resolve: x => userService.FindByLogin(x.GetArgument<string>("login")));

            FieldAsync<NonNullGraphType<UserType>, User>("userByEmail",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "email"}),
                resolve: x => userService.FindByEmail(x.GetArgument<string>("email")));

            // todo: find a way to get user id and add it here...
            FieldAsync<NonNullGraphType<UserType>, User>("me",
                resolve: x => userService.FindByLogin("cyberhck"));

            FieldAsync<NonNullGraphType<AvailabilityResultType>, AvailabilityResponse>("availabilityByUsername",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "username"}),
                resolve: x => availabilityService.AvailabilityByLogin(x.GetArgument<string>("username")));

            FieldAsync<NonNullGraphType<AvailabilityResultType>, AvailabilityResponse>("availabilityByEmail",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "email"}),
                resolve: x => availabilityService.AvailabilityByEmail(x.GetArgument<string>("email")));
        }
    }
}
