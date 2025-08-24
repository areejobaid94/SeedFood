using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.CloseDealStatuses
{
    [Table("CloseDealStatuses")]
    public class CloseDealStatus : Entity
    {

        public virtual string Status { get; set; }

    }
}