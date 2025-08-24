using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Branches;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.BranchAreas
{
	[Table("BranchAreas")]
    public class BranchArea : FullAuditedEntity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual DateTime CreationTime { get; set; }
		

		public virtual long AreaId { get; set; }
		
        [ForeignKey("AreaId")]
		public Area AreaFk { get; set; }
		
		public virtual long BranchId { get; set; }
		
        [ForeignKey("BranchId")]
		public Branch BranchFk { get; set; }
		
    }
}