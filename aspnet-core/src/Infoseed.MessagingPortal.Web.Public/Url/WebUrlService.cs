using Abp.Dependency;
using Infoseed.MessagingPortal.Configuration;
using Infoseed.MessagingPortal.Url;
using Infoseed.MessagingPortal.Web.Url;

namespace Infoseed.MessagingPortal.Web.Public.Url
{
    public class WebUrlService : WebUrlServiceBase, IWebUrlService, ITransientDependency
    {
        public WebUrlService(
            IAppConfigurationAccessor appConfigurationAccessor) :
            base(appConfigurationAccessor)
        {
        }

        public override string WebSiteRootAddressFormatKey => "App:WebSiteRootAddress";

        public override string ServerRootAddressFormatKey => "App:AdminWebSiteRootAddress";
    }
}