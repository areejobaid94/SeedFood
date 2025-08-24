using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace Infoseed.MessagingPortal.MultiTenancy.Payments.HyperPay
{
    public class HyperPayGatewayManager : MessagingPortalServiceBase, ITransientDependency
    {
        private readonly PayPalHttpClient _client;
        
        public HyperPayGatewayManager(HyperPayPaymentGatewayConfiguration configuration)
        {
            var environment = GetEnvironment(configuration);
            _client = new PayPalHttpClient(environment);
        }

        private PayPalEnvironment GetEnvironment(HyperPayPaymentGatewayConfiguration configuration)
        {
            switch (configuration.Environment)
            {
                case "sandbox":
                    {
                        return new SandboxEnvironment(configuration.ClientId, configuration.ClientSecret);
                    }
                case "live":
                    {
                        return new LiveEnvironment(configuration.ClientId, configuration.ClientSecret);
                    }
                default:
                    {
                        throw new ApplicationException("Unknown HyperPay environment");
                    }
            }
        }

        public async Task<string> CaptureOrderAsync(HyperPayExecutePaymentRequestInput input)
        {
            var request = new OrdersCaptureRequest(input.OrderId);
            request.RequestBody(new OrderActionRequest());

            var response = await _client.Execute(request);
            var payment = response.Result<Order>();
            if (payment.Status != "COMPLETED")
            {
                throw new UserFriendlyException(L("PaymentFailed"));
            }

            return payment.Id;
        }
    }
}