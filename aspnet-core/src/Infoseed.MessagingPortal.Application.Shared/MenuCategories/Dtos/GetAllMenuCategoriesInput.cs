using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.MenuCategories.Dtos
{
    public class GetAllMenuCategoriesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }



    }
}