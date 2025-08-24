namespace BotService.Models.API
{
    public class LocationInfoModel
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



        public int TenantId { get; set; }
        public int Id { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string DistrictName { get; set; }
        public int DistrictId { get; set; }
        public string LocationNameEn { get; set; }
        public decimal? DeliveryCost { get; set; }
        public string LocationName { get; set; }
        public string GoogleURL { get; set; }
        public int? ParentId { get; set; }
        public int LevelId { get; set; }


        public int BranchAreaId { get; set; }


        public string BranchAreaRes { get; set; }
        public string BranchAreaCor { get; set; }




        public int OrderCount { get; set; }



        public bool HasSubDistrict { get; set; }
        public int DeliveryCostId { get; set; }
    }
}
