using Infoseed.MessagingPortal.Editions.Dto;

namespace Infoseed.MessagingPortal.MultiTenancy.Payments.Dto
{
    public class PaymentInfoDto
    {
        public EditionSelectDto Edition { get; set; }

        public decimal AdditionalPrice { get; set; }

        public bool IsLessThanMinimumUpgradePaymentAmount()
        {
            return AdditionalPrice < MessagingPortalConsts.MinimumUpgradePaymentAmount;
        }
    }
}
