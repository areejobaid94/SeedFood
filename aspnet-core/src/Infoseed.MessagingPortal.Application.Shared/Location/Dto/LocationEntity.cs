using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Location.Dto
{
    
    public class LocationEntity 
    {
        public List<LocationModel> locationModel { get; set; }

        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string DistrictName { get; set; }
        public int TotalCount { get; set; }

    }
}
