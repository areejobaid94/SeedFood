using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ReceiptDetails.Dtos
{
    public class GetReceiptDetailForEditOutput
    {
        public CreateOrEditReceiptDetailDto ReceiptDetail { get; set; }

        public string ReceiptReceiptNumber { get; set; }

        public string AccountBillingBillID { get; set; }

    }
}