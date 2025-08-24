using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Currencies.Dtos
{
    public class CurrencyDto : EntityDto
    {
        public string CurrencyName { get; set; }

        public string ISOName { get; set; }

    }
}