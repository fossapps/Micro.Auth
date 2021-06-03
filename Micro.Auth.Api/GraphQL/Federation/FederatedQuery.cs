using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using GraphQL.Utilities.Federation;

namespace Micro.Auth.Api.GraphQL.Federation
{
    public class FederatedQuery : ObjectGraphType
    {
        public FederatedQuery()
        {
            Field<NonNullGraphType<ServiceGraphType>>().Name("_service").Resolve(x => new { });
                        Field<NonNullGraphType<ListGraphType<EntityType>>>()
                .Name("_entities")
                .Argument<NonNullGraphType<ListGraphType<NonNullGraphType<AnyScalarGraphType>>>>("representations")
                .ResolveAsync(context =>
                {
                    var representations = context.GetArgument<List<Dictionary<string, object>>>("representations");
                    var results = new List<Task<object>>();

                    foreach (var representation in representations)
                    {
                        var typeName = representation["__typename"].ToString();
                        var type = context.Schema.AllTypes[typeName];

                        if (type != null)
                        {
                            // execute resolver
                            var resolver = type.GetMetadata<IFederatedResolver>("__FedResolver__");
                            if (resolver != null)
                            {
                                var resolveContext = new FederatedResolveContext
                                {
                                    Arguments = representation,
                                    ParentFieldContext = context
                                };
                                var result = resolver.Resolve(resolveContext);
                                results.Add(result);
                            }
                            else
                            {
                                results.Add(Task.FromResult((object)representation));
                            }
                        }
                        else
                        {
                            // otherwise return the representation
                            results.Add(Task.FromResult((object)representation));
                        }
                    }

                    var tasks = Task.WhenAll(results).ContinueWith(results => (object)results.Result);
                    tasks.ConfigureAwait(false);
                    return tasks;
                });
        }
    }
}