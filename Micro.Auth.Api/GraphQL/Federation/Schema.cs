using System;
using GraphQL.Utilities.Federation;

namespace Micro.Auth.Api.GraphQL.Federation
{
    public class Schema : global::GraphQL.Types.Schema
    {
        protected Schema(IServiceProvider services) : base(services)
        {
            RegisterType(typeof(AnyScalarGraphType));
            RegisterType(typeof(ServiceGraphType));
            RegisterType(typeof(EntityType));
            Directives.Register(new ExternalDirective());
            Directives.Register(new RequiresDirective());
            Directives.Register(new ProvidesDirective());
            Directives.Register(new KeyDirective());
        }
    }
}
