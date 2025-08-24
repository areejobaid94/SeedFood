using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAdditionsCategorys
{
    [Table("ItemAdditionsCategorys")]
    public class ItemAdditionsCategory : Entity<long>, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		public virtual string Name { get; set; }
		public virtual string NameEnglish { get; set; }

		public int Priority { get; set; }

		public int MenuType { get; set; }
		public int LanguageBotId { get; set; }
		public bool IsCondiments { get; set; }
		public bool IsDeserts { get; set; }
		public bool IsCrispy { get; set; }
		//public bool IsNon { get; set; }
	}
}
