using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Infoseed.MessagingPortal.Authorization.Users;
using Abp.Authorization.Users;

namespace Infoseed.MessagingPortal.Areas
{
	[Table("Areas")]
    public class Area : Entity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		[StringLength(AreaConsts.MaxAreaNameLength, MinimumLength = AreaConsts.MinAreaNameLength)]
		public virtual string AreaName { get; set; }
		
		[StringLength(AreaConsts.MaxAreaCoordinateLength, MinimumLength = AreaConsts.MinAreaCoordinateLength)]
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