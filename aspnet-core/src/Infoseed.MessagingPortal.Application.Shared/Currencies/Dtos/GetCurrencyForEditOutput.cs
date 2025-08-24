using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Currencies.Dtos
{
    public class GetCurrencyForEditOutput
    {
        public CreateOrEditCurrencyDto Currency { get; set; }

    }
}