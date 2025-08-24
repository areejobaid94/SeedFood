using Abp.Domain.Entities;
using Infoseed.MessagingPortal.ItemAdditionsCategorys;
using Infoseed.MessagingPortal.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAdditions
{
	[Table("ItemAdditions")]
	public class ItemAdditions : Entity<long>, IMayHaveTenant
	{
		public int? TenantId { get; set; }


		[Required]
		public virtual string Name { get; set; }
		public virtual string NameEnglish { get; set; }


		public virtual decimal? Price { get; set; }

		public virtual string SKU { get; set; }
		public virtual long? ItemId { get; set; }

		[ForeignKey("ItemId")]
		public Item ItemFk { get; set; }

		public int MenuType { get; set; }
		public int LanguageBotId { get; set; }

		public virtual long? ItemAdditionsCategoryId { get; set; }

		[ForeignKey("ItemAdditionsCategory")]
		public ItemAdditionsCategory ItemAdditionsCategoryFk { get; set; }


		public string ImageUri { get; set; }

		public decimal LoyaltyPoints { get; set; }
		public decimal OriginalLoyaltyPoints { get; set; }
		public bool IsOverrideLoyaltyPoints { get; set; }
		//public virtual string Note { get; set; }
	}
}