using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Cities.Dtos
{
    public class GetCityForEditOutput
    {
		public CreateOrEditCityDto City { get; set; }


    }
}