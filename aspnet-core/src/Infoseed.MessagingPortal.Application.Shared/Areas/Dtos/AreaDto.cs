
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Areas.Dtos
{
    public class AreaDto : EntityDto<long>
    {
		public string AreaName { get; set; }

		public string AreaCoordinate { get; set; }

        public string AreaNameEnglish { get; set; }

        public string AreaCoordinateEnglish { get; set; }

        public virtual string BranchID { get; set; }

        public virtual int? UserId { get; set; }

        public virtual int RestaurantsType { get; set; }

        public virtual bool IsAssginToAllUser { get; set; }
        public virtual bool IsAvailableBranch { get; set; }

        public virtual bool IsRestaurantsTypeAll { get; set; }
        public virtual double? Latitude { get; set; }
        public virtual double? Longitude { get; set; }
        public virtual string SettingJson { get; set; }

        public string UserIds { get; set; }

        public string Url { get; set; }
    }
}