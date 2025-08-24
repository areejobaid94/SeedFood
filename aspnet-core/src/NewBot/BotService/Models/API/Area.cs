using System.ComponentModel.DataAnnotations;

namespace BotService.Models.API
{
    public class Area
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }

        public virtual string AreaName { get; set; }

        public virtual string AreaCoordinate { get; set; }

        public virtual string BranchID { get; set; }
        public virtual int? UserId { get; set; }

        public virtual bool IsAssginToAllUser { get; set; }
        public virtual bool IsAvailableBranch { get; set; }

        public virtual int RestaurantsType { get; set; }


        public virtual string AreaNameEnglish { get; set; }

        public virtual string AreaCoordinateEnglish { get; set; }

        public virtual bool IsRestaurantsTypeAll { get; set; }

        public virtual double? Latitude { get; set; }
        public virtual double? Longitude { get; set; }
        public virtual string SettingJson { get; set; }
        public virtual string UserIds { get; set; }
    }
}
