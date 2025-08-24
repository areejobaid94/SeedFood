using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DeliveryOrderDetails.Dtos
{
    public class GetAllDeliveryOrderDetailsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}