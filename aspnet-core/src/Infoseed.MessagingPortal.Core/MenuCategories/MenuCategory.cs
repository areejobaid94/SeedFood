using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.MenuCategories
{
	[Table("ItemCategorys")]
    public class ItemCategory : Entity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		[StringLength(MenuCategoryConsts.MaxNameLength, MinimumLength = MenuCategoryConsts.MinNameLength)]
		public virtual string Name { get; set; }
		public virtual string NameEnglish { get; set; }

		public  bool IsDeleted { get; set; }

		public int Priority { get; set; }

		public int MenuType { get; set; }
		public int LanguageBotId { get; set; }


		public string logoImag { get; set; }
		public string bgImag { get; set; }
		public long MenuId { get; set; }
		
	}
}