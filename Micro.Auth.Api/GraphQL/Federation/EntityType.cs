using GraphQL.Types;
using Micro.Auth.Api.GraphQL.Types;

namespace Micro.Auth.Api.GraphQL.Federation
{
    public class EntityType : UnionGraphType
    {
        public EntityType(ISchema schema)
        {
            Name = "_Entity";
            Type<UserType>();
        }
    }
}
