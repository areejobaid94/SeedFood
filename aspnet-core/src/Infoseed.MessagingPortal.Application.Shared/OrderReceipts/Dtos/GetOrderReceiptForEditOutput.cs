using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.OrderReceipts.Dtos
{
    public class GetOrderReceiptForEditOutput
    {
		public CreateOrEditOrderReceiptDto OrderReceipt { get; set; }

		public string OrderOrderRemarks { get; set;}


    }
}