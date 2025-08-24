using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.PaymentMethods
{
    [Table("PaymentMethods")]
    public class PaymentMethod : Entity
    {

        [Required]
        [StringLength(PaymentMethodConsts.MaxPaymnetMethodLength, MinimumLength = PaymentMethodConsts.MinPaymnetMethodLength)]
        public virtual string PaymnetMethod { get; set; }

    }
}