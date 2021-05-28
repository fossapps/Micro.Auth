using System.Collections.Generic;
using GraphQL.Types;
using Micro.Auth.Business.RefreshTokens;
using Micro.Auth.Storage;
using User = Micro.Auth.Business.Users.User;

namespace Micro.Auth.Api.GraphQL.Types
{
    public sealed class UserType : ObjectGraphType<User>
    {
        public UserType(ISessionService sessionService)
        {
            Name = "User";
            Field("id", x => x.Id).Description("user id");
            Field("username", x => x.UserName).Description("username");
            Field("email", x => x.Email).Description("email");
            Field("email_confirmed", x => x.EmailConfirmed).Description("email confirmed");
            Field("lockout_end", x => x.LockoutEnd, true).Description("point at which lockout ends");
            FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<RefreshTokenType>>>, IEnumerable<RefreshToken>>("sessions", resolve: x => sessionService.GetForUser(x.Source.Id));
        }
    }
}
