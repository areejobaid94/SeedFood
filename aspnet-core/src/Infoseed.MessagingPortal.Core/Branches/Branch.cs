using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.Branches
{
	[Table("Branches")]
    public class Branch : Entity<long> , IMayHaveTenant
    {
		public int? TenantId { get; set; }
			

		[Required]
		public virtual string Name { get; set; }

		public  decimal? DeliveryCost { get; set; }

		public virtual int RestaurantMenuType { get; set; }
		public virtual string RestaurantName { get; set; }
		public int BranchAreaId { get; set; }

		public int LocationID { get; set; }
	}
}