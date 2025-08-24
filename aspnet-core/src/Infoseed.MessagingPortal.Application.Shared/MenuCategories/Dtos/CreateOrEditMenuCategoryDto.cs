
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.MenuCategories.Dtos
{
    public class CreateOrEditMenuSubCategoryDto : EntityDto<long?>
    {

		[Required]
		[StringLength(MenuCategoryConsts.MaxNameLength, MinimumLength = MenuCategoryConsts.MinNameLength)]
		public  string Name { get; set; }
		public  long ItemCategoryId { get; set; }
		public  string NameEnglish { get; set; }
		public int Priority { get; set; }
		public int MenuType { get; set; }
		public int LanguageBotId { get; set; }
		public string logoImag { get; set; }
		public string bgImag { get; set; }
		public long MenuId { get; set; }
		public bool IsDeleted { get; set; }
		public int TenantId { get; set; }
		public long CopiedFromId { get; set; }
		public bool IsNew { get; set; }
		public decimal? Price { get; set; }
		
		

	}
}