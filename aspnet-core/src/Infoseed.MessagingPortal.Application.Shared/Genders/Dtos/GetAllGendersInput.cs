using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Genders.Dtos
{
    public class GetAllGendersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }



    }
}