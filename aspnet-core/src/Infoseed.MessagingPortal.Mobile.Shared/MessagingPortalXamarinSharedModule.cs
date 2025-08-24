using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Infoseed.MessagingPortal
{
    [DependsOn(typeof(MessagingPortalClientModule), typeof(AbpAutoMapperModule))]
    public class MessagingPortalXamarinSharedModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.IsEnabled = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessagingPortalXamarinSharedModule).GetAssembly());
        }
    }
}