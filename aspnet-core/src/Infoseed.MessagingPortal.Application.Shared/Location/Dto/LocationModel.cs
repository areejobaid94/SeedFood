using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Location.Dto
{
    public class LocationModel : PagedAndSortedResultRequestDto
    {

        public int TenantId { get; set; }
        public int LocationId { get; set; }
        public int DeliveryCostId { get; set; }
        public bool IsAvailable { get; set; }

        public string CountryName { get; set; }
        public int CountryId { get; set; }


        public string CityName { get; set; }
        public int CityId { get; set; }
        public string AreaName { get; set; }
        public int? AreaId { get; set; }
        public string DistrictName { get; set; }
        public int DistrictId { get; set; }
        public string LocationNameEn { get; set; }
        public decimal? DeliveryCost { get; set; }
        public string LocationName { get; set; }
        public string GoogleURL { get; set; }
        public int? ParentId { get; set; }
        public int LevelId { get; set; }
        public int? BranchAreaId { get; set; }
        public string BranchAreaRes { get; set; }
        public string BranchAreaCor { get; set; }
        public bool? HasSubDistrict { get; set; }
    }
}
