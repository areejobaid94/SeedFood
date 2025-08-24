using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.TenantServices.Dtos
{
    public class CreateOrEditTenantServiceDto : EntityDto<int?>
    {

        [Required]
        public decimal ServiceFees { get; set; }

        public int? InfoSeedServiceId { get; set; }


        public int FirstNumberOfOrders { get; set; }
        public decimal FeesForFirstOrder { get; set; }

    }
}