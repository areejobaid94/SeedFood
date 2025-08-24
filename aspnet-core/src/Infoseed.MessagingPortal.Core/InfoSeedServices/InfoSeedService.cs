using Infoseed.MessagingPortal.ServiceTypes;
using Infoseed.MessagingPortal.ServiceStatuses;
using Infoseed.MessagingPortal.ServiceFrequencies;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.InfoSeedServices
{
    [Table("InfoSeedServices")]
    public class InfoSeedService : FullAuditedEntity
    {
        public InfoSeedService() : base()
        {

        }
        public InfoSeedService(decimal serviceFees, string serviceName
            , DateTime serviceCreationDate, DateTime serviceStoppingDate
            , string remarks, int serviceTypeId, int serviceStatusId
            , int serviceFrquencyId, int creatorUserId, string serviceId, int firstNumberOfOrders, int feesForFirstOrder) : base()
        {
            this.Remarks = remarks;
            this.ServiceFees = serviceFees;
            this.ServiceName = serviceName;
            this.ServiceStoppingDate = serviceStoppingDate;
            this.ServiceStatusId = serviceStatusId;
            this.ServiceTypeId = serviceTypeId;
            this.ServiceCreationDate = serviceCreationDate;
            this.ServiceFrquencyId = serviceFrquencyId;
            this.CreationTime = serviceCreationDate;
            this.CreatorUserId = creatorUserId;
            this.CreationTime = DateTime.Now;
            this.ServiceID = serviceId;
            this.IsFeesPerTransaction = false;

            this.FirstNumberOfOrders = firstNumberOfOrders;
            this.FeesForFirstOrder = feesForFirstOrder;

        }

        [Required]
        [StringLength(InfoSeedServiceConsts.MaxServiceIDLength, MinimumLength = InfoSeedServiceConsts.MinServiceIDLength)]
        public virtual string ServiceID { get; set; }

        public virtual decimal ServiceFees { get; set; }

        [Required]
        [StringLength(InfoSeedServiceConsts.MaxServiceNameLength, MinimumLength = InfoSeedServiceConsts.MinServiceNameLength)]
        public virtual string ServiceName { get; set; }

        public virtual DateTime ServiceCreationDate { get; set; }

        public virtual DateTime ServiceStoppingDate { get; set; }

        public virtual string Remarks { get; set; }

        public virtual bool IsFeesPerTransaction { get; set; }

        public virtual int ServiceTypeId { get; set; }

        [ForeignKey("ServiceTypeId")]
        public ServiceType ServiceTypeFk { get; set; }

        public virtual int ServiceStatusId { get; set; }

        [ForeignKey("ServiceStatusId")]
        public ServiceStatus ServiceStatusFk { get; set; }

        public virtual int ServiceFrquencyId { get; set; }

        [ForeignKey("ServiceFrquencyId")]
        public ServiceFrquency ServiceFrquencyFk { get; set; }

        public int FirstNumberOfOrders { get; set; }
        public decimal FeesForFirstOrder { get; set; }


    }
}