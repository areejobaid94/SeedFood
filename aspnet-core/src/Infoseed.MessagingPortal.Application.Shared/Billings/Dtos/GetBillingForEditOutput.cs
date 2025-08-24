using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Billings.Dtos
{
    public class GetBillingForEditOutput
    {
        public CreateOrEditBillingDto Billing { get; set; }

        public string CurrencyCurrencyName { get; set; }

    }
}