using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.AccountBillings.Dtos
{
    public class GetAccountBillingForEditOutput
    {
        public CreateOrEditAccountBillingDto AccountBilling { get; set; }

        public string InfoSeedServiceServiceName { get; set; }

        public string ServiceTypeServicetypeName { get; set; }

        public string CurrencyCurrencyName { get; set; }

        public string BillingBillingID { get; set; }

    }
}