using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using GraphQL.Utilities.Federation;
using Micro.Auth.Api.GraphQL.DataLoaders;
using Micro.Auth.Api.GraphQL.Directives.Extensions;
using Micro.Auth.Api.GraphQL.Federation;
using Micro.Auth.Api.GraphQL.Types;
using Micro.Auth.Api.Internal.UserData.Extensions;
using Micro.Auth.Business.Availability;
using Micro.Auth.Business.Users;
using Microsoft.AspNetCore.Http;

namespace Micro.Auth.Api.GraphQL
{
    public sealed class Query : FederatedQuery
    {
        public Query(IUserService userService, IAvailabilityService availabilityService, UserByIdDataLoader userLoader, IHttpContextAccessor context)
        {
            Field<UserType, User>().Name("user").Argument<NonNullGraphType<StringGraphType>>("id").ResolveAsync(
                x => userLoader.LoadAsync(x.GetArgument<string>("id"))).Authorize();

            FieldAsync<UserType, User>("userByLogin",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "login"}),
                resolve: x => userService.FindByLogin(x.GetArgument<string>("login"))).Authorize();

            FieldAsync<UserType, User>("userByEmail",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "email"}),
                resolve: x => userService.FindByEmail(x.GetArgument<string>("email"))).Authorize();

            FieldAsync<NonNullGraphType<UserType>, User>("me",
                resolve: x => userService.FindById(context.GetUserId())).Authorize();

            FieldAsync<NonNullGraphType<AvailabilityResultType>, AvailabilityResponse>("availabilityByUsername",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "username"}),
                resolve: x => availabilityService.AvailabilityByLogin(x.GetArgument<string>("username")));

            FieldAsync<NonNullGraphType<AvailabilityResultType>, AvailabilityResponse>("availabilityByEmail",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "email"}),
                resolve: x => availabilityService.AvailabilityByEmail(x.GetArgument<string>("email")));
        }
    }
}
