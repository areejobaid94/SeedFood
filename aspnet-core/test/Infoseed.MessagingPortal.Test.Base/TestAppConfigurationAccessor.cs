using Abp.Dependency;
using Abp.Reflection.Extensions;
using Microsoft.Extensions.Configuration;
using Infoseed.MessagingPortal.Configuration;

namespace Infoseed.MessagingPortal.Test.Base
{
    public class TestAppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        public IConfigurationRoot Configuration { get; }

        public TestAppConfigurationAccessor()
        {
            Configuration = AppConfigurations.Get(
                typeof(MessagingPortalTestBaseModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }
    }
}
