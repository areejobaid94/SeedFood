
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Genders.Dtos
{
    public class CreateOrEditGenderDto : EntityDto<long?>
    {

		[Required]
		[StringLength(GenderConsts.MaxNameLength, MinimumLength = GenderConsts.MinNameLength)]
		public string Name { get; set; }
		
		

    }
}