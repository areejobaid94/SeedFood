using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Cities.Dtos
{
    public class GetAllCitiesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }



    }
}