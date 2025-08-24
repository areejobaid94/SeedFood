using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Infoseed.MessagingPortal.DealStatuses;
using Infoseed.MessagingPortal.DealTypes;

namespace Infoseed.MessagingPortal.Forcasts
{
    [Table("Forcasts")]
    public class Forcats : Entity, IMayHaveTenant
    {

        public int? TenantId { get; set; }

        public virtual string UserName { get; set; }

        public virtual string CustomerName { get; set; }

        public virtual string DealName { get; set; }

        public virtual string ARR { get; set; }

        public virtual string OrderFees { get; set; }

        public virtual DateTime CloseDate { get; set; }

        public virtual string DealAge { get; set; }

        public virtual int DealStatusId { get; set; }

        [ForeignKey("DealStatusId")]
        public DealStatus DealStatusFk { get; set; }

        public virtual int DealTypeId { get; set; }

        [ForeignKey("DealTypeId")]
        public DealType DealTypeFk { get; set; }

     //   public virtual string UserName { get; set; }

        public virtual string TotalCommit { get; set; }

        public virtual string TotalClosed { get; set; }

        public virtual DateTime SubmitDate { get; set; }

    }
}