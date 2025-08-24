using Infoseed.MessagingPortal.ChatStatuses;
using Infoseed.MessagingPortal.ContactStatuses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.TenantInformation
{

    [Table("TenantInformations")]
    public class TenantInformation : FullAuditedEntity
    {
        
        public int? TenantId { get ; set; }

        [Required]
        public virtual DateTime StartDate { get; set; }

        [Required]
        public virtual DateTime EndDate { get; set; }


        public long CurrentOrderNumber { get; set; }
        public long CurrentBookingNumber { get; set; }

    }
}