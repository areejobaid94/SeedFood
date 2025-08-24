using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.Banks
{
    [Table("Banks")]
    public class Bank : Entity
    {

        [Required]
        [StringLength(BankConsts.MaxBankNameLength, MinimumLength = BankConsts.MinBankNameLength)]
        public virtual string BankName { get; set; }

    }
}