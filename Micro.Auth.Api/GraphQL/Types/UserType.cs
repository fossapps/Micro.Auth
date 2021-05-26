using GraphQL.Types;
using Micro.Auth.Business.Users;

namespace Micro.Auth.Api.GraphQL.Types
{
    public sealed class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Name = "User";
            Field("id", x => x.Id).Description("user id");
            Field("username", x => x.UserName).Description("username");
            Field("email", x => x.Email).Description("email");
            Field("email_confirmed", x => x.EmailConfirmed).Description("email confirmed");
            Field("lockout_end", x => x.LockoutEnd, true).Description("point at which lockout ends");
        }
    }
}
