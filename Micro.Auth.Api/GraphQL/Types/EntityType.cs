namespace Micro.Auth.Api.GraphQL.Types
{
    public class EntityType : Micro.GraphQL.Federation.Types.EntityType
    {
        public EntityType()
        {
            Type<UserType>();
            Type<RefreshTokenType>();
        }
    }
}
