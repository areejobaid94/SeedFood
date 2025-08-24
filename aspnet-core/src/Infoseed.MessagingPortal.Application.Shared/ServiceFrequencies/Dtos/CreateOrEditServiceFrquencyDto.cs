using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ServiceFrequencies.Dtos
{
    public class CreateOrEditServiceFrquencyDto : EntityDto<int?>
    {

        [Required]
        [StringLength(ServiceFrquencyConsts.MaxServiceFrequencyNameLength, MinimumLength = ServiceFrquencyConsts.MinServiceFrequencyNameLength)]
        public string ServiceFrequencyName { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreationDate { get; set; }

    }
}