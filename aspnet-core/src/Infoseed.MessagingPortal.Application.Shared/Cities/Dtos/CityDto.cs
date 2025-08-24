
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Cities.Dtos
{
    public class CityDto : EntityDto<long>
    {
		public string Name { get; set; }



    }
}