using GraphQL.Types;
using Micro.Auth.Business.Availability;

namespace Micro.Auth.Api.GraphQL.Types
{
    public sealed class AvailabilityResultType : ObjectGraphType<AvailabilityResponse>
    {
        public AvailabilityResultType()
        {
            Name = "AvailabilityResult";
            Field("available", x => x.Available).Description("if data is available");
        }
    }
}
