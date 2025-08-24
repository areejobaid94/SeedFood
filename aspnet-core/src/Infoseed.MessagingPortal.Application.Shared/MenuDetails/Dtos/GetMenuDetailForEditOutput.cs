using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.MenuDetails.Dtos
{
    public class GetMenuDetailForEditOutput
    {
		public CreateOrEditMenuDetailDto MenuDetail { get; set; }

		public string ItemItemName { get; set;}
		public string ItemItemNameEnglish { get; set; }

		public string MenuMenuName { get; set; }
		public string MenuMenuNameEnglish { get; set; }

		public string MenuItemStatusName { get; set;}


    }
}