
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Cities.Dtos
{
    public class CreateOrEditCityDto : EntityDto<long?>
    {

		[Required]
		[StringLength(CityConsts.MaxNameLength, MinimumLength = CityConsts.MinNameLength)]
		public string Name { get; set; }
		
		

    }
}