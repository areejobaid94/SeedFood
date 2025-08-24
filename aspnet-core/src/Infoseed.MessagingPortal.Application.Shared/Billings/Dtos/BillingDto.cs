using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Billings.Dtos
{
    public class BillingDto : EntityDto
    {
        public int Id { get; set; }
        public string BillingID { get; set; }

        public DateTime BillingDate { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime BillPeriodTo { get; set; }

        public DateTime BillPeriodFrom { get; set; }

        public DateTime DueDate { get; set; }

        public int CurrencyId { get; set; }
        public  int TenantServiceId { get; set; }

        public  bool IsPayed { get; set; }
        public  string BillingResponse { get; set; }

        public string Status { get; set; }
        public string CustomerId { get; set; }
        public string InvoiceJson { get; set; }


    }
}