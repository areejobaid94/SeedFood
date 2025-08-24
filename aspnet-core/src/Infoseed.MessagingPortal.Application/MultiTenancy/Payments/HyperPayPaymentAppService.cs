using Infoseed.MessagingPortal.MultiTenancy.Payments.HyperPay;
using Infoseed.MessagingPortal.MultiTenancy.Payments.HyperPay.Dto;
using Infoseed.MessagingPortal.MultiTenancy.Payments.Paypal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.MultiTenancy.Payments
{
    public class HyperPayPaymentAppService : MessagingPortalAppServiceBase, IHyperPayPaymentAppService
    {
        private readonly HyperPayGatewayManager _hyperPayGatewayManager;
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly HyperPayPaymentGatewayConfiguration _hyperPayPaymentGatewayConfiguration;

        public HyperPayPaymentAppService(
            HyperPayGatewayManager hyperPayGatewayManager,
            ISubscriptionPaymentRepository subscriptionPaymentRepository,
            HyperPayPaymentGatewayConfiguration HyperPayPaymentGatewayConfiguration)
        {
            _hyperPayGatewayManager = hyperPayGatewayManager;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _hyperPayPaymentGatewayConfiguration = HyperPayPaymentGatewayConfiguration;
        }
        public async Task ConfirmPayment(long paymentId, string hyperPayOrderId)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(paymentId);

            await _hyperPayGatewayManager.CaptureOrderAsync(
                new HyperPayExecutePaymentRequestInput(hyperPayOrderId)
            );

            payment.Gateway = SubscriptionPaymentGatewayType.HyperPay;
            payment.ExternalPaymentId = hyperPayOrderId;
            payment.SetAsPaid();
        }

        public HyperPayConfigurationDto GetConfiguration()
        {
            return new HyperPayConfigurationDto
            {
                ClientId = _hyperPayPaymentGatewayConfiguration.ClientId,
                DemoUsername = _hyperPayPaymentGatewayConfiguration.DemoUsername,
                DemoPassword = _hyperPayPaymentGatewayConfiguration.DemoPassword
            };
        }
    }
}
