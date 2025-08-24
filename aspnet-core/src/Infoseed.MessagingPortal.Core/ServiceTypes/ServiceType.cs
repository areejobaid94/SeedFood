using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.ServiceTypes
{
    [Table("ServiceTypes")]
    public class ServiceType : Entity
    {
        public ServiceType():base()
        {

        }
        public ServiceType(string serviceName,bool isEnabled,DateTime createdDate) :base()
        {
            this.ServicetypeName = serviceName;
            this.IsEnabled = IsEnabled;
            this.CreationDate = createdDate;
        }

        [Required]
        [StringLength(ServiceTypeConsts.MaxServicetypeNameLength, MinimumLength = ServiceTypeConsts.MinServicetypeNameLength)]
        public virtual string ServicetypeName { get; set; }

        public virtual bool IsEnabled { get; set; }

        public virtual DateTime CreationDate { get; set; }

    }
}