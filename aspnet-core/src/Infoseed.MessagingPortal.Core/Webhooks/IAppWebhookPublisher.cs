using System.Threading.Tasks;
using Infoseed.MessagingPortal.Authorization.Users;

namespace Infoseed.MessagingPortal.WebHooks
{
    public interface IAppWebhookPublisher
    {
        Task PublishTestWebhook();
    }
}
