using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.MultiTenancy;

namespace Infoseed.MessagingPortal.MultiTenancy.Dto
{
    public class TenantEditDto : EntityDto
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(TenantConsts.MaxNameLength)]
        public string Name { get; set; }

        [DisableAuditing]
        public string ConnectionString { get; set; }

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
        public bool IsD360Dialog { get; set; }

        public bool Integration { get; set; }

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

        public int InsideNumber { get; set; }
        public int OutsideNumber { get; set; }

        public TenantBotLocalEunm BotLocal { get; set; }

        public virtual string Website { get; set; }


        public virtual string fileUrl { get; set; }
        public string AccessToken { get; set; }
        public string WhatsAppAccountID { get; set; }
        public string WhatsAppAppID { get; set; }
        public int? BotTemplateId { get; set; }
        public int BIDailyLimit { get; set; }
        public string CurrencyCode { get; set; }
        public string TimeZoneId { get; set; }
        public bool IsPreOrder { get; set; }
        public bool IsPickup { get; set; }
        public bool IsDelivery { get; set; }
        public string ZohoCustomerId { get; set; }
        public int ReminderBookingHour { get; set; }
        public int BookingCapacity { get; set; }

        public bool IsPaidInvoice { get; set; } = true;

        public bool IsCaution { get; set; } = false;

        public int CautionDays { get; set; }
        public int WarningDays { get; set; }
        public string UnAvailableBookingDates { get; set; }
        public string ConfirmRequestText { get; set; }
        public string RejectRequestText { get; set; }
        public string Currency { get; set; }

        public string ClientIpAddress { get; set; }


        public int DailyLimit { get; set; }
        public string MerchantID { get; set; }

        public string FacebookPageId { get; set; }
        public string FacebookAccessToken { get; set; }
        public string InstagramId { get; set; }
        public string InstagramAccessToken { get; set; }
        public string CatalogueLink { get; set; }
        public string BusinessId { get; set; }
        public string CatalogueAccessToken { get; set; }
        public string DeliveryType { get; set; }
        public string CareemAccessToken { get; set; }
    }
}