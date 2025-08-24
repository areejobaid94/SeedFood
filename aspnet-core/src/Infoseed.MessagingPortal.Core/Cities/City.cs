using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.Cities
{
	[Table("Cities")]
    public class City : Entity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		[StringLength(CityConsts.MaxNameLength, MinimumLength = CityConsts.MinNameLength)]
		public virtual string Name { get; set; }
		

    }
}