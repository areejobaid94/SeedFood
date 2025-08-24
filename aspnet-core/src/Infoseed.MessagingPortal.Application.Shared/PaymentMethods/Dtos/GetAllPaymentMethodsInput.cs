using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.PaymentMethods.Dtos
{
    public class GetAllPaymentMethodsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

    }
}