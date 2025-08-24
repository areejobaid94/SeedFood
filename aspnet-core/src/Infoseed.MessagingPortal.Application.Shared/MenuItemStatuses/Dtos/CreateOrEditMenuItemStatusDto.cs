
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.MenuItemStatuses.Dtos
{
    public class CreateOrEditMenuItemStatusDto : EntityDto<long?>
    {

		[Required]
		[StringLength(MenuItemStatusConsts.MaxNameLength, MinimumLength = MenuItemStatusConsts.MinNameLength)]
		public string Name { get; set; }
		
		

    }
}