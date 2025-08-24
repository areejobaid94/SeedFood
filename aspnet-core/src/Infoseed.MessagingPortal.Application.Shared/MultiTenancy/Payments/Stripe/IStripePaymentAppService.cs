using System.Threading.Tasks;
using Abp.Application.Services;
using Infoseed.MessagingPortal.MultiTenancy.Payments.Dto;
using Infoseed.MessagingPortal.MultiTenancy.Payments.Stripe.Dto;

namespace Infoseed.MessagingPortal.MultiTenancy.Payments.Stripe
{
    public interface IStripePaymentAppService : IApplicationService
    {
        Task ConfirmPayment(StripeConfirmPaymentInput input);

        StripeConfigurationDto GetConfiguration();

        Task<SubscriptionPaymentDto> GetPaymentAsync(StripeGetPaymentInput input);

        Task<string> CreatePaymentSession(StripeCreatePaymentSessionInput input);
    }
}