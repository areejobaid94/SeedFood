using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.PaymentMethods.Dtos
{
    public class CreateOrEditPaymentMethodDto : EntityDto<int?>
    {

        [Required]
        [StringLength(PaymentMethodConsts.MaxPaymnetMethodLength, MinimumLength = PaymentMethodConsts.MinPaymnetMethodLength)]
        public string PaymnetMethod { get; set; }

    }
}