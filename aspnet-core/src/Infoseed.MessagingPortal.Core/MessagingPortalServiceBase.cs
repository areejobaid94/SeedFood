using Abp;

namespace Infoseed.MessagingPortal
{
    /// <summary>
    /// This class can be used as a base class for services in this application.
    /// It has some useful objects property-injected and has some basic methods most of services may need to.
    /// It's suitable for non domain nor application service classes.
    /// For domain services inherit <see cref="MessagingPortalDomainServiceBase"/>.
    /// For application services inherit MessagingPortalAppServiceBase.
    /// </summary>
    public abstract class MessagingPortalServiceBase : AbpServiceBase
    {
        protected MessagingPortalServiceBase()
        {
            LocalizationSourceName = MessagingPortalConsts.LocalizationSourceName;
        }
    }
}