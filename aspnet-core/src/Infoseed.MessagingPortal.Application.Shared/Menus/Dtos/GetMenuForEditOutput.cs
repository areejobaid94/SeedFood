using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Menus.Dtos
{
    public class GetMenuForEditOutput
    {
		public CreateOrEditMenuDto Menu { get; set; }

		//public string MenuItemStatusName { get; set;}

		//public string MenuCategoryName { get; set;}


    }
}