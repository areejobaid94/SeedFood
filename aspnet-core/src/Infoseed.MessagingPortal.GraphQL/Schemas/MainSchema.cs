using Abp.Dependency;
using GraphQL;
using GraphQL.Types;
using Infoseed.MessagingPortal.Queries.Container;

namespace Infoseed.MessagingPortal.Schemas
{
    public class MainSchema : Schema, ITransientDependency
    {
        public MainSchema(IDependencyResolver resolver) :
            base(resolver)
        {
            Query = resolver.Resolve<QueryContainer>();
        }
    }
}