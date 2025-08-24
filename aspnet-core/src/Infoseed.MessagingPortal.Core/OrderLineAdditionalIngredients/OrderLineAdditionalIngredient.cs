using Infoseed.MessagingPortal.Orders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.OrderLineAdditionalIngredients
{
	[Table("OrderLineAdditionalIngredients")]
    public class OrderLineAdditionalIngredient : Entity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual string Remarks { get; set; }
		
		public virtual decimal? Total { get; set; }
		
		public virtual int? Quantity { get; set; }
		
		public virtual decimal? UnitPrice { get; set; }
		

		public virtual long? OrderId { get; set; }
		
        [ForeignKey("OrderId")]
		public Order OrderFk { get; set; }
		
    }
}