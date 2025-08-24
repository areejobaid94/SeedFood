using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.ReceiptDetails.Dtos
{
    public class ReceiptDetailDto : EntityDto
    {
        public string BillingNumber { get; set; }

        public DateTime BillDateFrom { get; set; }

        public DateTime BillDateTo { get; set; }

        public string ServiceName { get; set; }

        public decimal? BillAmount { get; set; }

        public decimal? OpenAmount { get; set; }

        public string CurrencyName { get; set; }

        public int ReceiptId { get; set; }

        public int? AccountBillingId { get; set; }

    }
}