using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DeliveryOrderDetails.Dtos
{
    public class DeliveryOrderDetailsDto : EntityDto<long>
	{
        public int? TenantId { get; set; }

       
        public int FromLocationId { get; set; }
        public string FromAddress { get; set; }
        public string FromGoogleURL { get; set; }

        public int ToLocationId { get; set; }
        public string ToAddress { get; set; }
        public string ToGoogleURL { get; set; }

        public decimal DeliveryCost { get; set; }
        public string DeliveryCostString { get; set; }

        public int OrderId { get; set; }


        public virtual string FromLocationDescribation { get; set; }
        public virtual string ToLocationDescribation { get; set; }
        public virtual string OrderDescribation { get; set; }


        public virtual string FromLongitudeLatitude { get; set; }
        public virtual string ToLongitudeLatitude { get; set; }

    }
}