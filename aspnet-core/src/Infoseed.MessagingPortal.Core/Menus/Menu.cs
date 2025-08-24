using Infoseed.MessagingPortal.MenuItemStatuses;
using Infoseed.MessagingPortal.MenuCategories;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Infoseed.MessagingPortal.CaptionBot;

namespace Infoseed.MessagingPortal.Menus
{
	[Table("Menus")]
    public class Menu : Entity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		[StringLength(MenuConsts.MaxMenuNameLength, MinimumLength = MenuConsts.MinMenuNameLength)]
		public virtual string MenuName { get; set; }
		
		public virtual string MenuDescription { get; set; }

		public virtual string MenuNameEnglish { get; set; }

		public virtual string MenuDescriptionEnglish { get; set; }


		public virtual DateTime? EffectiveTimeFrom { get; set; }
		
		public virtual DateTime? EffectiveTimeTo { get; set; }
		
		public virtual decimal? Tax { get; set; }
		
		[StringLength(MenuConsts.MaxImageUriLength, MinimumLength = MenuConsts.MinImageUriLength)]
		public virtual string ImageUri { get; set; }

		public int Priority { get; set; }

        public RestaurantsTypeEunm RestaurantsType { get; set; }

		public int LanguageBotId { get; set; }

		public virtual string ImageBgUri { get; set; }
		public MenuTypeEnum MenuTypeId { get; set; }

		public virtual string AreaIds { get; set; }
		//public LanguageBot LanguageBot { get; set; }

		//public virtual long? MenuItemStatusId { get; set; }

		//      [ForeignKey("MenuItemStatusId")]
		//public MenuItemStatus MenuItemStatusFk { get; set; }

		//public virtual long? MenuCategoryId { get; set; }

		//[ForeignKey("MenuCategoryId")]
		//public MenuCategory MenuCategoryFk { get; set; }

	}
}