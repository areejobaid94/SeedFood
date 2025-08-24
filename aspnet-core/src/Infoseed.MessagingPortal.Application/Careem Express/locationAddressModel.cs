using System.Collections.Generic;


namespace Infoseed.MessagingPortal.Careem_Express
{

    public class locationAddressModel
    {
        public int DistricId { get; set; }
        public string Distric { get; set; }

        public int AreaId { get; set; }
        public string Area { get; set; }

        public int CityId { get; set; }
        public string City { get; set; }

        public string Country { get; set; }


        public string Address { get; set; }

        public decimal? DeliveryCostAfter { get; set; }
        public decimal? DeliveryCostBefor { get; set; }

        public bool isOrderOfferCost { get; set; }

        public int LocationId { get; set; }
        public string LocationAreaName { get; set; }

        public string Route { get; set; }


        public string AddressEnglish { get; set; }


        public string AreaName { get; set; }
        public string AreaNameEnglish { get; set; }



        public string AreaCoordinate { get; set; }

        public string AreaCoordinateEnglish { get; set; }

        public List<string> List { get; set; }

    }

}
