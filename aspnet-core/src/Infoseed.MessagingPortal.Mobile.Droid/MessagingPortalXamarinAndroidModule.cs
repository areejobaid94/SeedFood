using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Infoseed.MessagingPortal
{
    [DependsOn(typeof(MessagingPortalXamarinSharedModule))]
    public class MessagingPortalXamarinAndroidModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessagingPortalXamarinAndroidModule).GetAssembly());
        }
    }
}