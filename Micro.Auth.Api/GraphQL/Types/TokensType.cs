using Micro.Auth.Business.Sessions;

namespace Micro.Auth.Api.GraphQL.Types
{
    public sealed class TokensType : Micro.GraphQL.Federation.ObjectGraphType<LoginSuccessResponse>
    {
        public TokensType()
        {
            Name = "Token";
            Field("jwt", x => x.Jwt).Description("JWT");
            Field("refreshToken", x => x.RefreshToken, true)
                .Description("Once JWT expires, use this token to create a new session");
        }
    }
}
