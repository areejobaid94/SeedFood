using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.TenantServices.Dtos
{
    public class TenantServiceDto : EntityDto
    {
        public decimal ServiceFees { get; set; }

        public int? InfoSeedServiceId { get; set; }


        public int FirstNumberOfOrders { get; set; }
        public decimal FeesForFirstOrder { get; set; }

    }
}