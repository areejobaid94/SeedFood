
using System;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.SealingReuest.Dto;

namespace Infoseed.MessagingPortal.Menus.Dtos
{
    public class MenuDto : EntityDto<long>
    {

		public virtual string MenuName { get; set; }

		public virtual string MenuDescription { get; set; }

		public virtual string MenuNameEnglish { get; set; }

		public virtual string MenuDescriptionEnglish { get; set; }

		public DateTime? EffectiveTimeFrom { get; set; }

		public DateTime? EffectiveTimeTo { get; set; }

		public decimal? Tax { get; set; }

		public string ImageUri { get; set; }

	    //public long? MenuItemStatusId { get; set; }

	    //public long? MenuCategoryId { get; set; }
		public int Priority { get; set; }
		public RestaurantsTypeEunm RestaurantsType { get; set; }

        public int RestaurantsTypeId
        {
            get { return (int)this.RestaurantsType; }
            set { this.RestaurantsType = (RestaurantsTypeEunm)value; }
        }


        public MenuTypeEnum MenuType { get; set; }

        public int MenuTypeId
        {
            get { return (int)this.MenuType; }
            set { this.MenuType = (MenuTypeEnum)value; }
        }


       // public MenuTypeEnum MenuTypeId { get; set; }
		public int LanguageBotId { get; set; }

		public  string ImageBgUri { get; set; }
		public string AreaIds { get; set; }
		
	}
}