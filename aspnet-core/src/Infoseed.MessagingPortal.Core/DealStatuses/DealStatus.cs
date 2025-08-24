using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.DealStatuses
{
    [Table("DealStatuses")]
    public class DealStatus : Entity
    {

        public virtual string Status { get; set; }

    }
}