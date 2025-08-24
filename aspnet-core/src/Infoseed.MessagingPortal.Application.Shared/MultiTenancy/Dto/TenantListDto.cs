using System;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Infoseed.MessagingPortal.MultiTenancy.Dto
{
    public class TenantListDto : EntityDto, IPassivable, IHasCreationTime
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }

        public string EditionDisplayName { get; set; }

        [DisableAuditing]
        public string ConnectionString { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public int? EditionId { get; set; }

        public bool IsInTrialPeriod { get; set; }
        public  int CustomerID { get; set; }
        public  string CustomerLegalName { get; set; }
        public  string CommercialName { get; set; }
        public  string LegalID { get; set; }
        public  int CustomerStatusID { get; set; }
        public  string  PhoneNumber { get; set; }
        public  string  Address { get; set; }
        public  string  Email { get; set; }
        public  string  ContactPersonName { get; set; }
        public  string ContactPersonPhoneNumber { get; set; }
        public  string ContactPersonEmail { get; set; }

        public string SmoochAppID { get; set; }

        public string SmoochSecretKey { get; set; }

        public string SmoochAPIAppID { get; set; }
        public string D360Key { get; set; }
        public string ServicesName { get; set; }

        public decimal TotalFeesServices { get; set; }

        public string DirectLineSecret { get; set; }
        public string botId { get; set; }
        public bool IsBotActive { get; set; }

        public string Image { get; set; }
        public TenantTypeEunm TenantType { get; set; }


        public virtual string Website { get; set; }
        public virtual string fileUrl { get; set; }
        public virtual string CurrencyCode { get; set; }
        public virtual string TimeZoneId { get; set; }

        public bool IsPaidInvoice { get; set; } = true;
        
    }
}