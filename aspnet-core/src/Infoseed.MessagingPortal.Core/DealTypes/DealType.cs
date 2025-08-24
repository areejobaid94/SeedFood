using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.DealTypes
{
    [Table("DealTypes")]
    public class DealType : Entity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Deal_Type { get; set; }

    }
}