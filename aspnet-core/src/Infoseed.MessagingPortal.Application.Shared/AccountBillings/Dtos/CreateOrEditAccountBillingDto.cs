using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.AccountBillings.Dtos
{
    public class CreateOrEditAccountBillingDto : EntityDto<int?>
    {

        [Required]
        [StringLength(AccountBillingConsts.MaxBillIDLength, MinimumLength = AccountBillingConsts.MinBillIDLength)]
        public string BillID { get; set; }

        [Required]
        public DateTime BillDateFrom { get; set; }

        [Required]
        public DateTime BillDateTo { get; set; }

        public decimal? OpenAmount { get; set; }

        public decimal? BillAmount { get; set; }

        public int? InfoSeedServiceId { get; set; }

        public int? ServiceTypeId { get; set; }

        public int? CurrencyId { get; set; }

        public int? BillingId { get; set; }
        public int Qty { get; set; }

    }
}