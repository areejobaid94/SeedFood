using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Infoseed.MessagingPortal.Configure;
using Infoseed.MessagingPortal.Startup;
using Infoseed.MessagingPortal.Test.Base;

namespace Infoseed.MessagingPortal.GraphQL.Tests
{
    [DependsOn(
        typeof(MessagingPortalGraphQLModule),
        typeof(MessagingPortalTestBaseModule))]
    public class MessagingPortalGraphQLTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddAndConfigureGraphQL();

            WindsorRegistrationHelper.CreateServiceProvider(IocManager.IocContainer, services);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessagingPortalGraphQLTestModule).GetAssembly());
        }
    }
}