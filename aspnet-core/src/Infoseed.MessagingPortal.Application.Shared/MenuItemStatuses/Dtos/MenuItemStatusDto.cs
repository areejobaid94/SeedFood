
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.MenuItemStatuses.Dtos
{
    public class MenuItemStatusDto : EntityDto<long>
    {
		public string Name { get; set; }



    }
}