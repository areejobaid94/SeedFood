using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models
{
    public class BranchModel
    {
		
		public int  Id { get; set; }
		public int? TenantId { get; set; }

		public  string Name { get; set; }

		public decimal? DeliveryCost { get; set; }

		public  int RestaurantMenuType { get; set; }
		public  string RestaurantName { get; set; }
		public int BranchAreaId { get; set; }
		public string BranchAreaName { get; set; }

		public int LocationID { get; set; }
	}
}
