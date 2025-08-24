using System.Threading.Tasks;
using Abp.Application.Services;

namespace Infoseed.MessagingPortal.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task DisableRecurringPayments();

        Task EnableRecurringPayments();
    }
}
