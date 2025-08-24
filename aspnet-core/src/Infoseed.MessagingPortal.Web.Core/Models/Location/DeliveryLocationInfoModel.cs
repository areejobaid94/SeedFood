using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Location
{
    public class DeliveryLocationInfoModel
    {

        public int TenantId { get; set; }
        public int Id { get; set; }
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }


     


        public string FromCityName { get; set; }
        public int FromCityId { get; set; }
        public string ToCityName { get; set; }
        public int ToCityId { get; set; }



        public string ToAreaName { get; set; }
        public int? ToAreaId { get; set; }
        public string FromAreaName { get; set; }
        public int? FromAreaId { get; set; }


        public string FromDistrictName { get; set; }
        public int FromDistrictId { get; set; }
        public string ToDistrictName { get; set; }
        public int ToDistrictId { get; set; }


        public string FromLocationNameEn { get; set; }
        public string FromLocationName { get; set; }
        public string ToLocationNameEn { get; set; }
        public string ToLocationName { get; set; }



        public decimal? DeliveryCost { get; set; }
     
        public string FromGoogleURL { get; set; }

        public string ToGoogleURL { get; set; }


        public int? ParentId { get; set; }
        public int LevelId { get; set; }


        public int BranchAreaId { get; set; }


        public string BranchAreaRes { get; set; }
        public string BranchAreaCor { get; set; }




        public int OrderCount { get; set; }
    }
}
