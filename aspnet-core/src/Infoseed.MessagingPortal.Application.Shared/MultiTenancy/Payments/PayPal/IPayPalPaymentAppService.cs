using System.Threading.Tasks;
using Abp.Application.Services;
using Infoseed.MessagingPortal.MultiTenancy.Payments.PayPal.Dto;

namespace Infoseed.MessagingPortal.MultiTenancy.Payments.PayPal
{
    public interface IPayPalPaymentAppService : IApplicationService
    {
        Task ConfirmPayment(long paymentId, string paypalOrderId);

        PayPalConfigurationDto GetConfiguration();
    }
}
