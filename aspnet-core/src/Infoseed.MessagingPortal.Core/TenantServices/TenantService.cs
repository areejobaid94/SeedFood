using Infoseed.MessagingPortal.InfoSeedServices;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.TenantServices
{
    [Table("TenantServices")]
    public class TenantService : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        public virtual decimal ServiceFees { get; set; }

        public virtual int? InfoSeedServiceId { get; set; }

        [ForeignKey("InfoSeedServiceId")]
        public InfoSeedService InfoSeedServiceFk { get; set; }

        public int FirstNumberOfOrders { get; set; }
        public decimal FeesForFirstOrder { get; set; }
    }
}