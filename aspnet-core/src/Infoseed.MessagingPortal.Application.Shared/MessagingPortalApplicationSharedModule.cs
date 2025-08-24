using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Infoseed.MessagingPortal
{
    [DependsOn(typeof(MessagingPortalCoreSharedModule))]
    public class MessagingPortalApplicationSharedModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessagingPortalApplicationSharedModule).GetAssembly());
        }
    }
}