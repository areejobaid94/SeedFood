using Abp.Domain.Services;

namespace Infoseed.MessagingPortal
{
    public abstract class MessagingPortalDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected MessagingPortalDomainServiceBase()
        {
            LocalizationSourceName = MessagingPortalConsts.LocalizationSourceName;
        }
    }
}
