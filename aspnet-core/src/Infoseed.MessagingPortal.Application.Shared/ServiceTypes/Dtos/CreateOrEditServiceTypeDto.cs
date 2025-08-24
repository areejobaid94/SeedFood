using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ServiceTypes.Dtos
{
    public class CreateOrEditServiceTypeDto : EntityDto<int?>
    {

        [Required]
        [StringLength(ServiceTypeConsts.MaxServicetypeNameLength, MinimumLength = ServiceTypeConsts.MinServicetypeNameLength)]
        public string ServicetypeName { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreationDate { get; set; }

    }
}