using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Infoseed.MessagingPortal
{
    public class MessagingPortalCoreSharedModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessagingPortalCoreSharedModule).GetAssembly());
        }
    }
}