using Abp.AspNetCore.Mvc.Views;

namespace Infoseed.MessagingPortal.Web.Views
{
    public abstract class MessagingPortalRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected MessagingPortalRazorPage()
        {
            LocalizationSourceName = MessagingPortalConsts.LocalizationSourceName;
        }
    }
}
