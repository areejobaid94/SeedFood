using System;
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.MultiTenancy;

namespace Infoseed.MessagingPortal.MultiTenancy.Dto
{
    public class CreateTenantInput
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(TenantConsts.TenancyNameRegex)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(TenantConsts.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string AdminEmailAddress { get; set; }

        [StringLength(AbpUserBase.MaxPasswordLength)]
        [DisableAuditing]
        public string AdminPassword { get; set; }

        [MaxLength(AbpTenantBase.MaxConnectionStringLength)]
        [DisableAuditing]
        public string ConnectionString { get; set; }

        public bool ShouldChangePasswordOnNextLogin { get; set; }

        public bool SendActivationEmail { get; set; }

        public int? EditionId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }


        public int CustomerID { get; set; }
        public string CustomerLegalName { get; set; }
        public string CommercialName { get; set; }
        public string LegalID { get; set; }
        public int CustomerStatusID { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        public string ContactPersonEmail { get; set; }

        public string SmoochAppID { get; set; }

        public string SmoochSecretKey { get; set; }

        public string SmoochAPIKeyID { get; set; }
        public string D360Key { get; set; }

        public string DirectLineSecret { get; set; }
        public string botId { get; set; }
        public bool IsBotActive { get; set; }


        public string Image { get; set; }

        public string ImagBg { get; set; }
        public TenantTypeEunm  TenantType{ get; set; }


        public decimal CostPerOrder { get; set; }
        public decimal SubscriptionFees { get; set; }
        public int DueDay { get; set; }


        public TenantBotLocalEunm BotLocal { get; set; }
        public virtual string Website { get; set; }
        public virtual string fileUrl { get; set; }

        public virtual bool IsBellOn { get; set; }
        public virtual bool IsBellContinues { get; set; }
        public virtual string BellSrc { get; set; }
        public virtual bool IsBooking { get; set; }
        public virtual int InsideNumber { get; set; }
        public virtual int OutsideNumber { get; set; }
        public string AccessToken { get; set; }
        public string WhatsAppAccountID { get; set; }
        public string WhatsAppAppID { get; set; }
        public int? BotTemplateId { get; set; }
        public int BIDailyLimit { get; set; }
        public int DailyLimit { get; set; } = 0;
        public string CurrencyCode { get; set; }
        public string TimeZone { get; set; }


        public bool IsPreOrder { get; set; }
        public bool IsPickup { get; set; }
        public bool IsDelivery { get; set; }
        public string ZohoCustomerId { get; set; }
        public bool IsPaidInvoice { get; set; } = true;
        public bool IsCaution { get; set; } = false;
        public int bookingCapacity { get; set; } = 0;
        public int reminderBookingHour { get; set; } = 0;

        public string ConfirmRequestText { get; set; }
        public string RejectRequestText { get; set; }

        public int CautionDays { get; set; }
        public int WarningDays { get; set; }

        public bool IsD360Dialog { get; set; }
        public string CatalogueLink { get; set; }
        public string BusinessId { get; set; }
        public string CatalogueAccessToken { get; set; }
    }

}