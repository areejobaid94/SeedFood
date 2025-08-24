using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ServiceStatuses.Dtos
{
    public class CreateOrEditServiceStatusDto : EntityDto<int?>
    {

        [Required]
        [StringLength(ServiceStatusConsts.MaxServiceStatusNameLength, MinimumLength = ServiceStatusConsts.MinServiceStatusNameLength)]
        public string ServiceStatusName { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreationDate { get; set; }

    }
}