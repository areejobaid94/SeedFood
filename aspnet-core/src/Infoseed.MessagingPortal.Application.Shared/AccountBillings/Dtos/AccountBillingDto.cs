using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.AccountBillings.Dtos
{
    public class AccountBillingDto : EntityDto
    {
        public string BillID { get; set; }

        public DateTime BillDateFrom { get; set; }

        public DateTime BillDateTo { get; set; }

        public decimal? OpenAmount { get; set; }

        public decimal? BillAmount { get; set; }

        public int? InfoSeedServiceId { get; set; }

        public int? ServiceTypeId { get; set; }

        public int? CurrencyId { get; set; }

        public int? BillingId { get; set; }

        public int? TenantId { get; set; }

        public int Qty { get; set; }


    }
}