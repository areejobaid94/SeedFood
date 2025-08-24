
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Areas.Dtos
{
    public class CreateOrEditAreaDto : EntityDto<long?>
    {

		[Required]
		[StringLength(AreaConsts.MaxAreaNameLength, MinimumLength = AreaConsts.MinAreaNameLength)]
		public string AreaName { get; set; }
		
		
		[StringLength(AreaConsts.MaxAreaCoordinateLength, MinimumLength = AreaConsts.MinAreaCoordinateLength)]
		public string AreaCoordinate { get; set; }

		public virtual string BranchID { get; set; }

		public virtual int? UserId { get; set; }

		public virtual int RestaurantsType { get; set; }

		public virtual bool IsAssginToAllUser { get; set; }
		public virtual bool IsAvailableBranch { get; set; }


		public virtual string AreaNameEnglish { get; set; }

		public virtual string AreaCoordinateEnglish { get; set; }

		public virtual bool IsRestaurantsTypeAll { get; set; }
		public virtual double? Latitude { get; set; }
		public virtual double? Longitude { get; set; }


		public virtual string  UserIds { get; set; }
	}
}