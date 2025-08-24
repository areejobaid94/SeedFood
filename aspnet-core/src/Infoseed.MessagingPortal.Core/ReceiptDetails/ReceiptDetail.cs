using Infoseed.MessagingPortal.Receipts;
using Infoseed.MessagingPortal.AccountBillings;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.ReceiptDetails
{
    [Table("ReceiptDetails")]
    public class ReceiptDetail : FullAuditedEntity
    {

        [StringLength(ReceiptDetailConsts.MaxBillingNumberLength, MinimumLength = ReceiptDetailConsts.MinBillingNumberLength)]
        public virtual string BillingNumber { get; set; }

        [Required]
        public virtual DateTime BillDateFrom { get; set; }

        [Required]
        public virtual DateTime BillDateTo { get; set; }

        [Required]
        [StringLength(ReceiptDetailConsts.MaxServiceNameLength, MinimumLength = ReceiptDetailConsts.MinServiceNameLength)]
        public virtual string ServiceName { get; set; }

        public virtual decimal? BillAmount { get; set; }

        public virtual decimal? OpenAmount { get; set; }

        [Required]
        [StringLength(ReceiptDetailConsts.MaxCurrencyNameLength, MinimumLength = ReceiptDetailConsts.MinCurrencyNameLength)]
        public virtual string CurrencyName { get; set; }

        public virtual int ReceiptId { get; set; }

        [ForeignKey("ReceiptId")]
        public Receipt ReceiptFk { get; set; }

        public virtual int? AccountBillingId { get; set; }

        [ForeignKey("AccountBillingId")]
        public AccountBilling AccountBillingFk { get; set; }

    }
}