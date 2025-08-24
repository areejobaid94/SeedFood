
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.OrderStatuses.Dtos
{
    public class OrderStatusDto : EntityDto<long>
    {
		public string Name { get; set; }



    }
}