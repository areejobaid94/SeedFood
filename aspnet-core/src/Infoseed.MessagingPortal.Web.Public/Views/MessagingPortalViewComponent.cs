using Abp.AspNetCore.Mvc.ViewComponents;

namespace Infoseed.MessagingPortal.Web.Public.Views
{
    public abstract class MessagingPortalViewComponent : AbpViewComponent
    {
        protected MessagingPortalViewComponent()
        {
            LocalizationSourceName = MessagingPortalConsts.LocalizationSourceName;
        }
    }
}