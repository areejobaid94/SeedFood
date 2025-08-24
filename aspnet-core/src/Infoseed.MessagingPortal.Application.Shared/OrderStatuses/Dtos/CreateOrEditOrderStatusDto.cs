
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.OrderStatuses.Dtos
{
    public class CreateOrEditOrderStatusDto : EntityDto<long?>
    {

		[Required]
		[StringLength(OrderStatusConsts.MaxNameLength, MinimumLength = OrderStatusConsts.MinNameLength)]
		public string Name { get; set; }
		
		

    }
}