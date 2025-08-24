using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.ServiceFrequencies
{
    [Table("ServiceFrquencies")]
    public class ServiceFrquency : Entity
    {
        public ServiceFrquency():base()
        {

        }
        public ServiceFrquency(string serviceFrequencyName, bool isEnabled, DateTime createdDate) : base()
        {
            this.ServiceFrequencyName = serviceFrequencyName;
            this.IsEnabled = IsEnabled;
            this.CreationDate = createdDate;
        }

        [Required]
        [StringLength(ServiceFrquencyConsts.MaxServiceFrequencyNameLength, MinimumLength = ServiceFrquencyConsts.MinServiceFrequencyNameLength)]
        public virtual string ServiceFrequencyName { get; set; }

        public virtual bool IsEnabled { get; set; }

        public virtual DateTime CreationDate { get; set; }

    }
}