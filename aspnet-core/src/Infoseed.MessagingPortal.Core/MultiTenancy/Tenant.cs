using System;
using System.ComponentModel.DataAnnotations;
using Abp.MultiTenancy;
using Abp.Timing;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Editions;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.MultiTenancy.Payments;

namespace Infoseed.MessagingPortal.MultiTenancy
{
    /// <summary>
    /// Represents a Tenant in the system.
    /// A tenant is a isolated customer for the application
    /// which has it's own users, roles and other application entities.
    /// </summary>
    public class Tenant : AbpTenant<User>
    {
        public const int MaxLogoMimeTypeLength = 64;

        //Can add application specific tenant properties here

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }

        public virtual Guid? CustomCssId { get; set; }

        public virtual Guid? LogoId { get; set; }

        [MaxLength(MaxLogoMimeTypeLength)]
        public virtual string LogoFileType { get; set; }

        public SubscriptionPaymentType SubscriptionPaymentType { get; set; }

        protected Tenant()
        {

        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {

        }

        public virtual bool HasLogo()
        {
            return LogoId != null && LogoFileType != null;
        }

        public void ClearLogo()
        {
            LogoId = null;
            LogoFileType = null;
        }

        public void UpdateSubscriptionDateForPayment(PaymentPeriodType paymentPeriodType, EditionPaymentType editionPaymentType)
        {
            switch (editionPaymentType)
            {
                case EditionPaymentType.NewRegistration:
                case EditionPaymentType.BuyNow:
                    {
                        SubscriptionEndDateUtc = Clock.Now.ToUniversalTime().AddDays((int)paymentPeriodType);
                        break;
                    }
                case EditionPaymentType.Extend:
                    ExtendSubscriptionDate(paymentPeriodType);
                    break;
                case EditionPaymentType.Upgrade:
                    if (HasUnlimitedTimeSubscription())
                    {
                        SubscriptionEndDateUtc = Clock.Now.ToUniversalTime().AddDays((int)paymentPeriodType);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private void ExtendSubscriptionDate(PaymentPeriodType paymentPeriodType)
        {
            if (SubscriptionEndDateUtc == null)
            {
                throw new InvalidOperationException("Can not extend subscription date while it's null!");
            }

            if (IsSubscriptionEnded())
            {
                SubscriptionEndDateUtc = Clock.Now.ToUniversalTime();
            }

            SubscriptionEndDateUtc = SubscriptionEndDateUtc.Value.AddDays((int)paymentPeriodType);
        }

        private bool IsSubscriptionEnded()
        {
            return SubscriptionEndDateUtc < Clock.Now.ToUniversalTime();
        }

        public int CalculateRemainingHoursCount()
        {
            return SubscriptionEndDateUtc != null
                ? (int)(SubscriptionEndDateUtc.Value - Clock.Now.ToUniversalTime()).TotalHours //converting it to int is not a problem since max value ((DateTime.MaxValue - DateTime.MinValue).TotalHours = 87649416) is in range of integer.
                : 0;
        }

        public bool HasUnlimitedTimeSubscription()
        {
            return SubscriptionEndDateUtc == null;
        }

        public virtual int CustomerID { get; set; }
        public virtual string CustomerLegalName { get; set; }
        public virtual string CommercialName { get; set; }
        public virtual string LegalID { get; set; }
        public virtual int CustomerStatusID { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Address { get; set; }
        public virtual string Email { get; set; }
        public virtual string Website { get; set; }
        public virtual string ContactPersonName { get; set; }
        public virtual string ContactPersonPhoneNumber { get; set; }
        public virtual string ContactPersonEmail { get; set; }
        public string SmoochAppID { get; set; }

        public string SmoochSecretKey { get; set; }

        public string SmoochAPIKeyID { get; set; }

        public string D360Key { get; set; }
        public string DirectLineSecret { get; set; }
        public string botId { get; set; }
        public bool IsBotActive { get; set; }

        public string Image { get; set; }
        public string ImageBg { get; set; }
        public TenantTypeEunm TenantType { get; set; }

        public decimal CostPerOrder { get; set; }
        public decimal SubscriptionFees { get; set; }
        public int DueDay { get; set; }

        public bool IsBellOn { get; set; }
        public bool IsBellContinues { get; set; }
        public string BellSrc { get; set; }


        public bool IsBooking { get; set; }

        public  int InsideNumber { get; set; }
        public  int OutsideNumber { get; set; }

        public TenantBotLocalEunm BotLocal { get; set; }

        public virtual string fileUrl { get; set; }

        public string AccessToken { get; set; }
        public string WhatsAppAccountID { get; set; }

        public string WhatsAppAppID { get; set; }
       public int? BotTemplateId { get; set; }
        public int BIDailyLimit { get; set; }
        public int DailyLimit { get; set; } = 0;
        public string CurrencyCode { get; set; }
        public string TimeZoneId { get; set; }

        public bool IsPreOrder { get; set; }
        public bool IsPickup { get; set; }
        public bool IsDelivery { get; set; }
        public string ZohoCustomerId { get; set; } = "";
        public bool IsPaidInvoice { get; set; } = true;
        public int ReminderBookingHour { get; set; }
        public int BookingCapacity { get; set; }
        public bool IsCaution { get; set; } = false;


        public int CautionDays { get; set; }
        public int WarningDays { get; set; }
        public string UnAvailableBookingDates { get; set; }
        public string ConfirmRequestText { get; set; }
        public string RejectRequestText { get; set; }

        public bool IsD360Dialog { get; set; }

        public string MerchantID { get; set; }

        public string FacebookPageId { get; set; }
        public string FacebookAccessToken { get; set; }
        public string InstagramId { get; set; }
        public string InstagramAccessToken { get; set; }
        public bool? Integration { get; set; }
        public string CatalogueLink { get; set; }
        public string BusinessId { get; set; }
        public string CatalogueAccessToken { get; set; }
    }
}