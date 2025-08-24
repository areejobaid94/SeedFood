using Microsoft.Extensions.Configuration;

namespace Infoseed.MessagingPortal.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
