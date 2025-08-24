using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Currencies.Dtos
{
    public class CreateOrEditCurrencyDto : EntityDto<int?>
    {

        [Required]
        [StringLength(CurrencyConsts.MaxCurrencyNameLength, MinimumLength = CurrencyConsts.MinCurrencyNameLength)]
        public string CurrencyName { get; set; }

        [Required]
        [StringLength(CurrencyConsts.MaxISONameLength, MinimumLength = CurrencyConsts.MinISONameLength)]
        public string ISOName { get; set; }

    }
}