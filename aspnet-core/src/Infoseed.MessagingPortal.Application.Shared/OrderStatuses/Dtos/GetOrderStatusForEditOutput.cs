using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.OrderStatuses.Dtos
{
    public class GetOrderStatusForEditOutput
    {
		public CreateOrEditOrderStatusDto OrderStatus { get; set; }


    }
}