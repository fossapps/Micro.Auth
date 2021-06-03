using System;
using System.Threading.Tasks;
using GraphQL.Types;
using GraphQL.Utilities.Federation;

namespace Micro.Auth.Api.GraphQL.Federation
{
    public class FederatedObjectGraphType<TSource> : ObjectGraphType<TSource>
    {
        protected void ResolveReferenceAsync(Func<FederatedResolveContext, Task<TSource>> resolver)
        {
            ResolveReferenceAsync(new FuncFederatedResolver<TSource>(resolver));
        }

        protected void Key(string fields)
        {
            this.BuildAstMeta("key", fields);
        }

        private void ResolveReferenceAsync(IFederatedResolver resolver)
        {
            // Metadata[FederatedSchemaBuilder.RESOLVER_METADATA_FIELD] = resolver;
            Metadata["__FedResolver__"] = resolver;
        }
    }
}
