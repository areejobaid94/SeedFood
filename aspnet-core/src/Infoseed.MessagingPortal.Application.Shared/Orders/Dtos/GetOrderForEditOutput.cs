using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class GetOrderForEditOutput
    {
		public CreateOrEditOrderDto Order { get; set; }

		public string DeliveryChangeDeliveryServiceProvider { get; set;}

		public string BranchName { get; set;}

		public string CustomerCustomerName { get; set;}

		public string OrderStatusName { get; set;}


    }
}