using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.PaymentMethods.Dtos
{
    public class PaymentMethodDto : EntityDto
    {
        public string PaymnetMethod { get; set; }

    }
}