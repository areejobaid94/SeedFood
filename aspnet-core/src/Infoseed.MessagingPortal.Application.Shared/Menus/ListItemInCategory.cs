using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Menus
{
   public  class ListItemInCategory
    {
		public long itemId { get; set; }
		public string ImageUri { get; set; }

		public string CategoryNames { get; set; }
		
		public bool IsInService { get; set; }
		public string ItemName { get; set; }
		public string ItemDescription { get; set; }
		public decimal? Price { get; set; }
		public int Priority { get; set; }
	}
}
