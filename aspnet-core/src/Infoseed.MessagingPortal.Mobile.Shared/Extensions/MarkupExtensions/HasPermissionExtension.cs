using System;
using Infoseed.MessagingPortal.Core;
using Infoseed.MessagingPortal.Core.Dependency;
using Infoseed.MessagingPortal.Services.Permission;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Infoseed.MessagingPortal.Extensions.MarkupExtensions
{
    [ContentProperty("Text")]
    public class HasPermissionExtension : IMarkupExtension
    {
        public string Text { get; set; }
        
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (ApplicationBootstrapper.AbpBootstrapper == null || Text == null)
            {
                return false;
            }

            var permissionService = DependencyResolver.Resolve<IPermissionService>();
            return permissionService.HasPermission(Text);
        }
    }
}