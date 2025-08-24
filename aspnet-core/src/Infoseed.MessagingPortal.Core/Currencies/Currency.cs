using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.Currencies
{
    [Table("Currencies")]
    public class Currency : CreationAuditedEntity
    {

        [Required]
        [StringLength(CurrencyConsts.MaxCurrencyNameLength, MinimumLength = CurrencyConsts.MinCurrencyNameLength)]
        public virtual string CurrencyName { get; set; }

        [Required]
        [StringLength(CurrencyConsts.MaxISONameLength, MinimumLength = CurrencyConsts.MinISONameLength)]
        public virtual string ISOName { get; set; }

    }
}