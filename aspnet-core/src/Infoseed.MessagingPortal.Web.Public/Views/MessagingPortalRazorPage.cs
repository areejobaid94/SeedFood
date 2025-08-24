using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Infoseed.MessagingPortal.Web.Public.Views
{
    public abstract class MessagingPortalRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected MessagingPortalRazorPage()
        {
            LocalizationSourceName = MessagingPortalConsts.LocalizationSourceName;
        }
    }
}
