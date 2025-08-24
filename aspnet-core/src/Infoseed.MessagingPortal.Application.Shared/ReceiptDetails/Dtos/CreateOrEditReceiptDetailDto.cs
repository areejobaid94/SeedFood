using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ReceiptDetails.Dtos
{
    public class CreateOrEditReceiptDetailDto : EntityDto<int?>
    {

        [StringLength(ReceiptDetailConsts.MaxBillingNumberLength, MinimumLength = ReceiptDetailConsts.MinBillingNumberLength)]
        public string BillingNumber { get; set; }

        [Required]
        public DateTime BillDateFrom { get; set; }

        [Required]
        public DateTime BillDateTo { get; set; }

        [Required]
        [StringLength(ReceiptDetailConsts.MaxServiceNameLength, MinimumLength = ReceiptDetailConsts.MinServiceNameLength)]
        public string ServiceName { get; set; }

        public decimal? BillAmount { get; set; }

        public decimal? OpenAmount { get; set; }

        [Required]
        [StringLength(ReceiptDetailConsts.MaxCurrencyNameLength, MinimumLength = ReceiptDetailConsts.MinCurrencyNameLength)]
        public string CurrencyName { get; set; }

        public int ReceiptId { get; set; }

        public int? AccountBillingId { get; set; }

    }
}