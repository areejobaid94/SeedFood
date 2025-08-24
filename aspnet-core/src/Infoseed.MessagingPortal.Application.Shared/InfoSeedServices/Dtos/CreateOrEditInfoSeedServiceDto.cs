using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.InfoSeedServices.Dtos
{
    public class CreateOrEditInfoSeedServiceDto : EntityDto<int?>
    {

        [Required]
        [StringLength(InfoSeedServiceConsts.MaxServiceIDLength, MinimumLength = InfoSeedServiceConsts.MinServiceIDLength)]
        public string ServiceID { get; set; }

        public decimal ServiceFees { get; set; }

        [Required]
        [StringLength(InfoSeedServiceConsts.MaxServiceNameLength, MinimumLength = InfoSeedServiceConsts.MinServiceNameLength)]
        public string ServiceName { get; set; }

        public DateTime ServiceCreationDate { get; set; }

        public DateTime ServiceStoppingDate { get; set; }

        public string Remarks { get; set; }

        public bool IsFeesPerTransaction { get; set; }

        public int ServiceTypeId { get; set; }

        public int ServiceStatusId { get; set; }

        public int ServiceFrquencyId { get; set; }

        public int FirstNumberOfOrders { get; set; }
        public decimal FeesForFirstOrder { get; set; }

    }
}