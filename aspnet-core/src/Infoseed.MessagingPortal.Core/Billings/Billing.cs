using Infoseed.MessagingPortal.Currencies;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Infoseed.MessagingPortal.TenantServices;

namespace Infoseed.MessagingPortal.Billings
{
    [Table("Billings")]
    public class Billing : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(BillingConsts.MaxBillingIDLength, MinimumLength = BillingConsts.MinBillingIDLength)]
        public virtual string BillingID { get; set; }

        [Required]
        public virtual DateTime BillingDate { get; set; }

        [Required]
        public virtual decimal TotalAmount { get; set; }

        [Required]
        public virtual DateTime BillPeriodTo { get; set; }

        public virtual DateTime BillPeriodFrom { get; set; }

        [Required]
        public virtual DateTime DueDate { get; set; }

        public virtual int CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public Currency CurrencyFk { get; set; }

        //public virtual int TenantServiceId { get; set; }

        //[ForeignKey("TenantServiceId")]
        //public TenantService TenantServiceFk { get; set; }

        public virtual bool IsPayed { get; set; }

        public virtual string BillingResponse { get; set; }

    }
}