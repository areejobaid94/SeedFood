using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Currencies.Dtos
{
    public class GetAllCurrenciesForExcelInput
    {
        public string Filter { get; set; }

        public string CurrencyNameFilter { get; set; }

        public string ISONameFilter { get; set; }

    }
}