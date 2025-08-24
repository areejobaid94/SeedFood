using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Menus
{
	public class ListItem
	{
		public CategorysModle categorysModle { get; set; }

		public long Id { get; set; }
		public string ImageUri { get; set; }
		public bool IsInService { get; set; }

		public string ItemName { get; set; }
		public string ItemNameEnglish { get; set; }
		public string ItemDescription { get; set; }
		public string ItemDescriptionEnglish { get; set; }


		public decimal? Price { get; set; }
		public int Priority { get; set; }
		public virtual string SKU { get; set; }
		public List<ItemAdditionDto> itemAdditionDtos { get; set; }
	}
}
