using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Ctown
{
    public class ItemSubCategory
    {
		public int Id { get; set; }
		public int? TenantId { get; set; }

		public virtual string Name { get; set; }
		public virtual string NameEnglish { get; set; }
		public int Priority { get; set; }
		public bool IsDeleted { get; set; }

		public int ItemCategoryId { get; set; }

		public int MenuType { get; set; }
		public int LanguageBotId { get; set; }


		public string logoImag { get; set; }
		public string bgImag { get; set; }
	}
}
