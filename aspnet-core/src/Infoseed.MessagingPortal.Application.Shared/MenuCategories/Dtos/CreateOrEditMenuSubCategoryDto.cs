
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.MenuCategories.Dtos
{
    public class CreateOrEditMenuCategoryDto : EntityDto<long?>
    {

		[Required]
		[StringLength(MenuCategoryConsts.MaxNameLength, MinimumLength = MenuCategoryConsts.MinNameLength)]
		public  string Name { get; set; }
		public  string NameEnglish { get; set; }


		public int Priority { get; set; }

		public int MenuType { get; set; }

		public int LanguageBotId { get; set; }


		public string logoImag { get; set; }
		public string bgImag { get; set; }
		public long MenuId { get; set; }
		public List<CreateOrEditMenuSubCategoryDto> lstMenuSubCategoryDto { get; set; }

	}
}