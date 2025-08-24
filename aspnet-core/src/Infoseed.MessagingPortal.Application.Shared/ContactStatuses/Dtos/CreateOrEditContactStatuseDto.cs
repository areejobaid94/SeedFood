using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ContactStatuses.Dtos
{
    public class CreateOrEditContactStatuseDto : EntityDto<int?>
    {

        [Required]
        [StringLength(ContactStatuseConsts.MaxContactStatusNameLength, MinimumLength = ContactStatuseConsts.MinContactStatusNameLength)]
        public string ContactStatusName { get; set; }

        [Required]
        public bool IsEnabled { get; set; }

    }
}