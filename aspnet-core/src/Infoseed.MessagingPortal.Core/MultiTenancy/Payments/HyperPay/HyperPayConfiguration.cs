using Abp.Extensions;
using Microsoft.Extensions.Configuration;
using Infoseed.MessagingPortal.Configuration;

namespace Infoseed.MessagingPortal.MultiTenancy.Payments.HyperPay
{
    public class HyperPayPaymentGatewayConfiguration : IPaymentGatewayConfiguration
    {
        private readonly IConfigurationRoot _appConfiguration;

        public SubscriptionPaymentGatewayType GatewayType => SubscriptionPaymentGatewayType.HyperPay;

        public string Environment => _appConfiguration["Payment:HyperPay:Environment"];

        public string ClientId => _appConfiguration["Payment:HyperPay:ClientId"];

        public string ClientSecret => _appConfiguration["Payment:HyperPay:ClientSecret"];

        public string DemoUsername => _appConfiguration["Payment:HyperPay:DemoUsername"];

        public string DemoPassword => _appConfiguration["Payment:HyperPay:DemoPassword"];

        public bool IsActive => _appConfiguration["Payment:HyperPay:IsActive"].To<bool>();

        public bool SupportsRecurringPayments => false;
        
        public HyperPayPaymentGatewayConfiguration(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
        }
    }
}