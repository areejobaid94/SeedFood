using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Menus;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Infoseed.MessagingPortal.Items;

namespace Infoseed.MessagingPortal.OrderDetails
{
	[Table("OrderDetails")]
	public class OrderDetail : FullAuditedEntity<long>, IMayHaveTenant
	{
		public int? TenantId { get; set; }


		public virtual int? Quantity { get; set; }

		public virtual decimal? UnitPrice { get; set; }

		public virtual decimal? Total { get; set; }

		public virtual decimal? Discount { get; set; }

		public virtual decimal? TotalAfterDiscunt { get; set; }

		public virtual decimal? Tax { get; set; }

		public virtual decimal? TotalAfterTax { get; set; }

		[Required]
		public virtual DateTime CreationTime { get; set; }

		public virtual DateTime? DeletionTime { get; set; }

		public virtual DateTime? LastModificationTime { get; set; }

		public bool IsCondiments { get; set; }
		public bool IsDeserts { get; set; }
		public bool IsCrispy { get; set; }


	public virtual long? OrderId { get; set; }
		
        [ForeignKey("OrderId")]
		public Order OrderFk { get; set; }
		
		public virtual long? ItemId { get; set; }
		
        [ForeignKey("ItemId")]
		public Item ItemsFk { get; set; }


		
    }
}