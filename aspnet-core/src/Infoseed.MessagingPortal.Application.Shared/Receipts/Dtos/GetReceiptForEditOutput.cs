using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Receipts.Dtos
{
    public class GetReceiptForEditOutput
    {
        public CreateOrEditReceiptDto Receipt { get; set; }

        public string BankBankName { get; set; }

        public string PaymentMethodPaymnetMethod { get; set; }

    }
}