using System;
using GraphQL.Types;
using Micro.Auth.Api.GraphQL.Types;
using Micro.Auth.Business.Users;

namespace Micro.Auth.Api.GraphQL
{
    public class Query : ObjectGraphType
    {
        public Query()
        {
            Field<UserType>("user", resolve: x=> new User
            {
                Email = "gautam.nishchal@gmail.com",
                Id = "123123123123",
                EmailConfirmed = true,
                UserName = "cyberhck",
                LockoutEnd = DateTime.Now
            });
        }
    }
}
