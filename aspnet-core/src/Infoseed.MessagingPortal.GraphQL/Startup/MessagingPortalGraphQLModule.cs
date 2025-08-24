using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Infoseed.MessagingPortal.Startup
{
    [DependsOn(typeof(MessagingPortalCoreModule))]
    public class MessagingPortalGraphQLModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessagingPortalGraphQLModule).GetAssembly());
        }

        public override void PreInitialize()
        {
            base.PreInitialize();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }
    }
}