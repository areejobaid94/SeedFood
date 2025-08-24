using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.OrderDetails.Dtos
{
    public class GetOrderDetailForEditOutput
    {
		public CreateOrEditOrderDetailDto OrderDetail { get; set; }

		public string OrderOrderRemarks { get; set;}

        public string ItemName { get; set; }
        public string ItemNameEnglish { get; set; }


    }
}