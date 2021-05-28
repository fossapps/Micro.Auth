using GraphQL.Types;
using Micro.Auth.Business.Common;

namespace Micro.Auth.Api.GraphQL.Types
{
    public sealed class ResultType : ObjectGraphType<Result>
    {
        public ResultType()
        {
            Name = "MutationResult";
            Field("success", x => x.Success).Description("describes success");
        }
    }
}
