using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Banks.Dtos
{
    public class CreateOrEditBankDto : EntityDto<int?>
    {

        [Required]
        [StringLength(BankConsts.MaxBankNameLength, MinimumLength = BankConsts.MinBankNameLength)]
        public string BankName { get; set; }

    }
}