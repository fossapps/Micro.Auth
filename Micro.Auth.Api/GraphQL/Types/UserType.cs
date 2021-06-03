using System.Collections.Generic;
using GraphQL.DataLoader;
using GraphQL.Types;
using Micro.Auth.Api.GraphQL.DataLoaders;
using Micro.Auth.Api.GraphQL.Federation;
using Micro.Auth.Storage;
using User = Micro.Auth.Business.Users.User;

namespace Micro.Auth.Api.GraphQL.Types
{
    public sealed class UserType : FederatedObjectGraphType<User>
    {
        public UserType(SessionByUserDataLoader sessionLoader)
        {
            Name = "User";
            Key("id");
            Field("id", x => x.Id).Description("user id");
            Field("username", x => x.UserName).Description("username");
            Field("email", x => x.Email).Description("email");
            Field("email_confirmed", x => x.EmailConfirmed).Description("email confirmed");
            Field("lockout_end", x => x.LockoutEnd, true).Description("point at which lockout ends");
            Field<NonNullGraphType<ListGraphType<RefreshTokenType>>, IEnumerable<RefreshToken>>().Name("sessions")
                .ResolveAsync(x => sessionLoader.LoadAsync(x.Source.Id));

            ResolveReferenceAsync(async ctx =>
            {
                var id = ctx.Arguments["id"].ToString();
                return new User
                {
                    Id = id,
                };
            });
        }
    }
}
