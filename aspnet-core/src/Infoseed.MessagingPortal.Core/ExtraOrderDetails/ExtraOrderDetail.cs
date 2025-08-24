using Abp.Domain.Entities;
using Infoseed.MessagingPortal.OrderDetails;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.ExtraOrderDetails
{
	[Table("ExtraOrderDetail")]
	public class ExtraOrderDetail : Entity<long>, IMayHaveTenant
	{
		public int? TenantId { get; set; }
	
		public virtual string Name { get; set; }
		public virtual string NameEnglish { get; set; }
		public virtual int? Quantity { get; set; }

		public virtual decimal? UnitPrice { get; set; }

		public virtual decimal? Total { get; set; }

		public virtual long? OrderDetailId { get; set; }

		public virtual string SpecificationName { get; set; }
		public virtual string SpecificationNameEnglish { get; set; }
		public virtual int SpecificationUniqueId { get; set; }

		[ForeignKey("OrderDetailId")]
		public OrderDetail OrderDetailFk { get; set; }
	}
}