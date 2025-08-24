using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Infoseed.MessagingPortal
{
    [DependsOn(typeof(MessagingPortalXamarinSharedModule))]
    public class MessagingPortalXamarinIosModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessagingPortalXamarinIosModule).GetAssembly());
        }
    }
}