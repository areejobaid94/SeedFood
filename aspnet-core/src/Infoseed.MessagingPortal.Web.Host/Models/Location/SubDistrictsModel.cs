using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Location
{
    public class SubDistrictsModel
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
