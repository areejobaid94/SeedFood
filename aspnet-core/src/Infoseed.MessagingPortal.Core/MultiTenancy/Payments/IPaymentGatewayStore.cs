using System.Collections.Generic;

namespace Infoseed.MessagingPortal.MultiTenancy.Payments
{
    public interface IPaymentGatewayStore
    {
        List<PaymentGatewayModel> GetActiveGateways();
    }
}
