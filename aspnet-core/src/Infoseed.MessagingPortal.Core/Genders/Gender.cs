using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.Genders
{
	[Table("Genders")]
    public class Gender : Entity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		[StringLength(GenderConsts.MaxNameLength, MinimumLength = GenderConsts.MinNameLength)]
		public virtual string Name { get; set; }
		

    }
}