using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.ReceiptDetails.Dtos
{
    public class GetAllReceiptDetailsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string BillingNumberFilter { get; set; }

        public DateTime? MaxBillDateFromFilter { get; set; }
        public DateTime? MinBillDateFromFilter { get; set; }

        public DateTime? MaxBillDateToFilter { get; set; }
        public DateTime? MinBillDateToFilter { get; set; }

        public string ServiceNameFilter { get; set; }

        public decimal? MaxBillAmountFilter { get; set; }
        public decimal? MinBillAmountFilter { get; set; }

        public decimal? MaxOpenAmountFilter { get; set; }
        public decimal? MinOpenAmountFilter { get; set; }

        public string CurrencyNameFilter { get; set; }

        public string ReceiptReceiptNumberFilter { get; set; }

        public string AccountBillingBillIDFilter { get; set; }

        public int? ReceiptIdFilter { get; set; }
    }
}