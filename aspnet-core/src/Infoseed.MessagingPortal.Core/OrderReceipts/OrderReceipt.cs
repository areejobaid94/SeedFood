using Infoseed.MessagingPortal.Orders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.OrderReceipts
{
	[Table("OrderReceipts")]
    public class OrderReceipt : FullAuditedEntity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual DateTime? OrderTime { get; set; }
		
		public virtual int? OrderAmount { get; set; }
		
		public virtual decimal? OrderDiscount { get; set; }
		
		public virtual decimal? TotalAfterDiscunt { get; set; }
		
		public virtual bool IsCashReceived { get; set; }
		
		[Required]
		public virtual DateTime CreationTime { get; set; }
		
		public virtual DateTime? DeletionTime { get; set; }
		
		public virtual DateTime? LastModificationTime { get; set; }
		

		public virtual long? OrderId { get; set; }
		
        [ForeignKey("OrderId")]
		public Order OrderFk { get; set; }
		
    }
}