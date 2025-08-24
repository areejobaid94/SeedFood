using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Infoseed.MessagingPortal.TenantServices.Dtos;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Billings.Dtos
{
    public class CreateOrEditBillingDto : EntityDto<int?>
    {

   
        public string BillingID { get; set; }

    
        public DateTime BillingDate { get; set; }

       
        public decimal TotalAmount { get; set; }

 
        public DateTime BillPeriodTo { get; set; }

        public DateTime BillPeriodFrom { get; set; }


        public DateTime DueDate { get; set; }

        public int CurrencyId { get; set; }
        public int TenantId { get; set; }

        public string BillingResponse { get; set; }
        public  bool IsPayed { get; set; }



        //////////////
        ///      
        public List<TenantServiceModalDto> TenantService { get; set; }
    }
}