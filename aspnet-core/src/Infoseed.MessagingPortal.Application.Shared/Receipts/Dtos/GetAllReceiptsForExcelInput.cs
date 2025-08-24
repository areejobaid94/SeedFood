using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Receipts.Dtos
{
    public class GetAllReceiptsForExcelInput
    {
        public string Filter { get; set; }

        public string ReceiptNumberFilter { get; set; }

        public DateTime? MaxReceiptDateFilter { get; set; }
        public DateTime? MinReceiptDateFilter { get; set; }

        public string PaymentReferenceNumberFilter { get; set; }

        public string BankBankNameFilter { get; set; }

        public string PaymentMethodPaymnetMethodFilter { get; set; }

    }
}