
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace Infoseed.MessagingPortal.Menus.Dtos
{
    public class CreateOrEditMenuDto : EntityDto<long?>
    {

        [Required]
        [StringLength(MenuConsts.MaxMenuNameLength, MinimumLength = MenuConsts.MinMenuNameLength)]
		public virtual string MenuName { get; set; }

		public virtual string MenuDescription { get; set; }

		public virtual string MenuNameEnglish { get; set; }

		public virtual string MenuDescriptionEnglish { get; set; }


		public DateTime? EffectiveTimeFrom { get; set; }
		
		
		public DateTime? EffectiveTimeTo { get; set; }
		
		
		public decimal? Tax { get; set; }
		
		
		//[StringLength(MenuConsts.MaxImageUriLength, MinimumLength = MenuConsts.MinImageUriLength)]
		public string ImageUri { get; set; }

		public int Priority { get; set; }
		public RestaurantsTypeEunm RestaurantsType { get; set; }
		public int LanguageBotId { get; set; }

		public  string ImageBgUri { get; set; }
		public MenuTypeEnum MenuTypeId { get; set; }
		public virtual string AreaIds { get; set; }
		//public long? MenuItemStatusId { get; set; }

		//		 public long? MenuCategoryId { get; set; }


	}
}