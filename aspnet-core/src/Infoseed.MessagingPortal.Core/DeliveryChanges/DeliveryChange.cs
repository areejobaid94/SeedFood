using Infoseed.MessagingPortal.Areas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.DeliveryChanges
{
	[Table("DeliveryChanges")]
    public class DeliveryChange : FullAuditedEntity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual decimal? Charges { get; set; }
		
		[StringLength(DeliveryChangeConsts.MaxDeliveryServiceProviderLength, MinimumLength = DeliveryChangeConsts.MinDeliveryServiceProviderLength)]
		public virtual string DeliveryServiceProvider { get; set; }
		
		[Required]
		public virtual DateTime CreationTime { get; set; }
		
		public virtual DateTime? DeletionTime { get; set; }
		
		public virtual DateTime? LastModificationTime { get; set; }
		

		public virtual long AreaId { get; set; }
		
        [ForeignKey("AreaId")]
		public Area AreaFk { get; set; }
		
    }
}