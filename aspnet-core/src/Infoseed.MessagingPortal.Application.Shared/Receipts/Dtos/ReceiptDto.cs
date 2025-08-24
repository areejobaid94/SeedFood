using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Receipts.Dtos
{
    public class ReceiptDto : EntityDto
    {
        public string ReceiptNumber { get; set; }

        public DateTime ReceiptDate { get; set; }

        public string PaymentReferenceNumber { get; set; }

        public int? BankId { get; set; }

        public int PaymentMethodId { get; set; }

    }
}