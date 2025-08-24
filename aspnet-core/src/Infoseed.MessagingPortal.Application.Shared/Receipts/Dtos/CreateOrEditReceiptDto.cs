using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Receipts.Dtos
{
    public class CreateOrEditReceiptDto : EntityDto<int?>
    {

        [Required]
        [StringLength(ReceiptConsts.MaxReceiptNumberLength, MinimumLength = ReceiptConsts.MinReceiptNumberLength)]
        public string ReceiptNumber { get; set; }

        [Required]
        public DateTime ReceiptDate { get; set; }

        [StringLength(ReceiptConsts.MaxPaymentReferenceNumberLength, MinimumLength = ReceiptConsts.MinPaymentReferenceNumberLength)]
        public string PaymentReferenceNumber { get; set; }

        public int? BankId { get; set; }

        public int PaymentMethodId { get; set; }

    }
}