using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.ServiceStatuses
{
    [Table("ServiceStatuses")]
    public class ServiceStatus : Entity
    {
        public ServiceStatus():base()
        {

        }
        public ServiceStatus(string serviceStatusName, bool isEnabled, DateTime createdDate) : base()
        {
            this.ServiceStatusName = serviceStatusName;
            this.IsEnabled = IsEnabled;
            this.CreationDate = createdDate;
        }

        [Required]
        [StringLength(ServiceStatusConsts.MaxServiceStatusNameLength, MinimumLength = ServiceStatusConsts.MinServiceStatusNameLength)]
        public virtual string ServiceStatusName { get; set; }

        public virtual bool IsEnabled { get; set; }

        public virtual DateTime CreationDate { get; set; }

    }
}