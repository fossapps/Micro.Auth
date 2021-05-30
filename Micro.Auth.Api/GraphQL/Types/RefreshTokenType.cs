using GraphQL.Types;
using Micro.Auth.Storage;

namespace Micro.Auth.Api.GraphQL.Types
{
    public sealed class RefreshTokenType : ObjectGraphType<RefreshToken>
    {
        public RefreshTokenType()
        {
            Name = "Session";
            Field("location", x => x.Location).Description("rough geo location");
            Field("useragent", x => x.Useragent).Description("useragent which was used to create this session");
            Field("ipaddress", x => x.IpAddress.ToString()).Description("IP address from which this session was created");
            Field("last_used", x => x.LastUsed).Description("Last usage of this token");
            Field("id", x => x.Id).Description("id of this session");
        }
    }
}
