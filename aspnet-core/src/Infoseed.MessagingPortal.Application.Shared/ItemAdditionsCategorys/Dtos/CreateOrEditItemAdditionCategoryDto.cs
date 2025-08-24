using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos
{
    public class CreateOrEditItemAdditionCategoryDto
    {
		public long Id { get; set; }
		public  string Name { get; set; }
		public  string NameEnglish { get; set; }

		public int Priority { get; set; }

		public int MenuType { get; set; }
		public int LanguageBotId { get; set; }
		public bool IsCondiments { get; set; }
		public bool IsDeserts { get; set; }
		public bool IsCrispy { get; set; }
		public bool IsNon { get; set; }

	}
}
