namespace Infoseed.MessagingPortal.MultiTenancy.Payments.HyperPay
{
    public class HyperPayExecutePaymentRequestInput
    {
        public string OrderId { get; set; }

        public HyperPayExecutePaymentRequestInput(string orderId)
        {
            OrderId = orderId;
        }
    }
}