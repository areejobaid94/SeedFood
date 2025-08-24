using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Location.Dto
{
    public class SubDistrictModel
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int LocationDeliveryCostId { get; set; }
        public string LocationNameEn { get; set; }
        public string LocationName { get; set; }
        public int CreatorUserId { get; set; }
        public string CreationTime { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public decimal DeliveryCost { get; set; }
        public string LongitudeAndLatitude { get; set; }

    }
}
