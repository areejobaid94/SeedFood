using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.OrderStatuses
{
	[Table("OrderStatuses")]
    public class OrderStatus : Entity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		[StringLength(OrderStatusConsts.MaxNameLength, MinimumLength = OrderStatusConsts.MinNameLength)]
		public virtual string Name { get; set; }
		

    }
}