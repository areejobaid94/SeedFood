using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.AccountBillings.Dtos
{
    public class GetAllAccountBillingsForExcelInput
    {
        public string Filter { get; set; }

        public string BillIDFilter { get; set; }

        public DateTime? MaxBillDateFromFilter { get; set; }
        public DateTime? MinBillDateFromFilter { get; set; }

        public DateTime? MaxBillDateToFilter { get; set; }
        public DateTime? MinBillDateToFilter { get; set; }

        public decimal? MaxOpenAmountFilter { get; set; }
        public decimal? MinOpenAmountFilter { get; set; }

        public decimal? MaxBillAmountFilter { get; set; }
        public decimal? MinBillAmountFilter { get; set; }

        public string InfoSeedServiceServiceNameFilter { get; set; }

        public string ServiceTypeServicetypeNameFilter { get; set; }

        public string CurrencyCurrencyNameFilter { get; set; }

        public string BillingBillingIDFilter { get; set; }

    }
}