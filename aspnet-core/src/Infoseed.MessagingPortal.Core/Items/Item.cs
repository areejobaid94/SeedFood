using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.MenuCategories;

namespace Infoseed.MessagingPortal.Items
{
	[Table("Items")]
    public class Item : FullAuditedEntity<long> , IMayHaveTenant
    {
		public int? TenantId { get; set; }
			
		
		
		public virtual string Ingredients { get; set; }
		
		[Required]
		[StringLength(ItemConsts.MaxItemNameLength, MinimumLength = ItemConsts.MinItemNameLength)]
		public virtual string ItemName { get; set; }
		public virtual string ItemDescription { get; set; }


		public virtual string ItemNameEnglish { get; set; }
		public virtual string ItemDescriptionEnglish { get; set; }

		public virtual string CategoryNames { get; set; }

		public virtual string CategoryNamesEnglish { get; set; }



		public virtual bool IsInService { get; set; }
		
		
		
		[Required]
		public virtual DateTime CreationTime { get; set; }
		
		public virtual DateTime? DeletionTime { get; set; }
		
		public virtual DateTime? LastModificationTime { get; set; }


		public virtual decimal? Price { get; set; }
		public virtual decimal? OldPrice { get; set; }

		public virtual string ImageUri { get; set; }

		public int Priority { get; set; }
		public virtual string SKU { get; set; }

		public virtual long? MenuId { get; set; }

		[ForeignKey("MenuId")]
		public Menu MenuFk { get; set; }

		public virtual long? ItemCategoryId { get; set; }

		[ForeignKey("ItemCategoryId")]
		public ItemCategory ItemCategoryFk { get; set; }

		public virtual long? ItemSubCategoryId { get; set; }
		
		public int MenuType { get; set; }
		public int LanguageBotId { get; set; }

		public string Barcode { get; set; }
		public string BarcodeImg { get; set; }
		public string AreaIds { get; set; }
		public bool IsQuantitative { get; set; }

        public bool IsLoyal { get; set; }
        public decimal LoyaltyPoints { get; set; }
        public decimal OriginalLoyaltyPoints { get; set; }
        public bool IsOverrideLoyaltyPoints { get; set; }
       // public long? LoyaltyDefinitionId { get; set; }
    }
}