using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.DeliveryChanges.Dtos
{
    public class GetDeliveryChangeForEditOutput
    {
		public CreateOrEditDeliveryChangeDto DeliveryChange { get; set; }

		public string AreaAreaName { get; set;}


    }
}