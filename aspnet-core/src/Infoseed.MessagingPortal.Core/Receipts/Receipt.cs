using Infoseed.MessagingPortal.Banks;
using Infoseed.MessagingPortal.PaymentMethods;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.Receipts
{
    [Table("Receipts")]
    public class Receipt : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(ReceiptConsts.MaxReceiptNumberLength, MinimumLength = ReceiptConsts.MinReceiptNumberLength)]
        public virtual string ReceiptNumber { get; set; }

        [Required]
        public virtual DateTime ReceiptDate { get; set; }

        [StringLength(ReceiptConsts.MaxPaymentReferenceNumberLength, MinimumLength = ReceiptConsts.MinPaymentReferenceNumberLength)]
        public virtual string PaymentReferenceNumber { get; set; }

        public virtual int? BankId { get; set; }

        [ForeignKey("BankId")]
        public Bank BankFk { get; set; }

        public virtual int PaymentMethodId { get; set; }

        [ForeignKey("PaymentMethodId")]
        public PaymentMethod PaymentMethodFk { get; set; }

    }
}