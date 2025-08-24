using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Infoseed.MessagingPortal.Authorization;

namespace Infoseed.MessagingPortal
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(MessagingPortalApplicationSharedModule),
        typeof(MessagingPortalCoreModule)
        )]
    public class MessagingPortalApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessagingPortalApplicationModule).GetAssembly());
        }
    }
}