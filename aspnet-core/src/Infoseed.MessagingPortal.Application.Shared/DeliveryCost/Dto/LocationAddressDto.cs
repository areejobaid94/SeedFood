using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DeliveryCost.Dto
{
    public class LocationAddressDto
    {
        public int LocationId { get; set; }
        public string Route { get; set; }
        public string Distric { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string AddressEnglish { get; set; }

        public long AreaId { get; set; }
        public string AreaName { get; set; }
        public string AreaNameEnglish { get; set; }

        public decimal DeliveryCostAfter { get; set; }
        public decimal DeliveryCostBefor { get; set; }

        public string AreaCoordinate { get; set; }

        public string AreaCoordinateEnglish { get; set; }
        public string LocationName { get; set; }

        public List<string> List { get; set; }

    }
}
