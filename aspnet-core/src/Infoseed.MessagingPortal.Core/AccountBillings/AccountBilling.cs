using Infoseed.MessagingPortal.InfoSeedServices;
using Infoseed.MessagingPortal.ServiceTypes;
using Infoseed.MessagingPortal.Currencies;
using Infoseed.MessagingPortal.Billings;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.AccountBillings
{
    [Table("AccountBillings")]
    public class AccountBilling : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(AccountBillingConsts.MaxBillIDLength, MinimumLength = AccountBillingConsts.MinBillIDLength)]
        public virtual string BillID { get; set; }

        [Required]
        public virtual DateTime BillDateFrom { get; set; }

        [Required]
        public virtual DateTime BillDateTo { get; set; }

        public virtual decimal? OpenAmount { get; set; }

        public virtual decimal? BillAmount { get; set; }

        public virtual int? InfoSeedServiceId { get; set; }

        [ForeignKey("InfoSeedServiceId")]
        public InfoSeedService InfoSeedServiceFk { get; set; }

        public virtual int? ServiceTypeId { get; set; }

        [ForeignKey("ServiceTypeId")]
        public ServiceType ServiceTypeFk { get; set; }

        public virtual int? CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public Currency CurrencyFk { get; set; }

        public virtual int? BillingId { get; set; }

        [ForeignKey("BillingId")]
        public Billing BillingFk { get; set; }

        public int Qty { get; set; }
    }
}