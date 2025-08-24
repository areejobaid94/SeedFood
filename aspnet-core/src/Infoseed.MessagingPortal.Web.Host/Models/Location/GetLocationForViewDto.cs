using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Location
{
    public class GetLocationForViewDto
    {
        public List<LocationInfoModel> locationInfoModel { get; set; }


        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string DistrictName { get; set; }

    }
}
