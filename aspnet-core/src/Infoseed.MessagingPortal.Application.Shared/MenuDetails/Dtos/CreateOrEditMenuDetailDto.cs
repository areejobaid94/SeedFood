
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.MenuDetails.Dtos
{
    public class CreateOrEditMenuDetailDto : EntityDto<long?>
    {

		public string Description { get; set; }
		
		
		public bool IsStandAlone { get; set; }
		
		
		public decimal? Price { get; set; }
		
		
		 public long ItemId { get; set; }
		 
		 		 public long MenuId { get; set; }
		 
		 		 public long? MenuItemStatusId { get; set; }
		 
		 
    }
}