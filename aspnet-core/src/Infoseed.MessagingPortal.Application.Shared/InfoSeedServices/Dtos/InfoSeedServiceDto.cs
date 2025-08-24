using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.InfoSeedServices.Dtos
{
    public class InfoSeedServiceDto : EntityDto
    {
        public string ServiceID { get; set; }

        public string ServiceName { get; set; }

        public DateTime ServiceCreationDate { get; set; }

        public DateTime ServiceStoppingDate { get; set; }

        public int ServiceTypeId { get; set; }

        public int ServiceStatusId { get; set; }

        public int ServiceFrquencyId { get; set; }


        public int FirstNumberOfOrders { get; set; }
        public decimal FeesForFirstOrder { get; set; }

    }
}