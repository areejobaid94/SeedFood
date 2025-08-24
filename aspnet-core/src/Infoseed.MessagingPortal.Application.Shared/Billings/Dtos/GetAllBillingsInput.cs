using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Billings.Dtos
{
    public class GetAllBillingsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string BillingIDFilter { get; set; }

        public DateTime? MaxBillingDateFilter { get; set; }
        public DateTime? MinBillingDateFilter { get; set; }

        public decimal? MaxTotalAmountFilter { get; set; }
        public decimal? MinTotalAmountFilter { get; set; }

        public DateTime? MaxBillPeriodToFilter { get; set; }
        public DateTime? MinBillPeriodToFilter { get; set; }

        public DateTime? MaxBillPeriodFromFilter { get; set; }
        public DateTime? MinBillPeriodFromFilter { get; set; }

        public DateTime? MaxDueDateFilter { get; set; }
        public DateTime? MinDueDateFilter { get; set; }

        public string CurrencyCurrencyNameFilter { get; set; }

    }
}