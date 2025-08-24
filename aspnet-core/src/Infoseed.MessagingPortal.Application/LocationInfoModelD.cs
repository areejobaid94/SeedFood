using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public  class LocationInfoModelD
    {
        public int TenantId { get; set; }
        public int LocationId { get; set; }
        public int Id { get; set; }
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


        public int BranchAreaId { get; set; }


        public string BranchAreaRes { get; set; }
        public string BranchAreaCor { get; set; }
    }
}
