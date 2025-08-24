using Infoseed.MessagingPortal.Genders;
using Infoseed.MessagingPortal.Cities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.Customers
{
	[Table("Customers")]
    public class Customer : FullAuditedEntity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		[StringLength(CustomerConsts.MaxCustomerNameLength, MinimumLength = CustomerConsts.MinCustomerNameLength)]
		public virtual string CustomerName { get; set; }
		
		[StringLength(CustomerConsts.MaxPhoneNumberLength, MinimumLength = CustomerConsts.MinPhoneNumberLength)]
		public virtual string PhoneNumber { get; set; }
		
		public virtual string CustomerAddress { get; set; }
		
		[Required]
		public virtual DateTime CreationTime { get; set; }
		
		public virtual DateTime? DeletionTime { get; set; }
		
		[Required]
		[StringLength(CustomerConsts.MaxEmailAddressLength, MinimumLength = CustomerConsts.MinEmailAddressLength)]
		public virtual string EmailAddress { get; set; }
		
		[Required]
		public virtual bool IsActive { get; set; }
		
		[Required]
		public virtual bool IsDeleted { get; set; }
		

		public virtual long? GenderId { get; set; }
		
        [ForeignKey("GenderId")]
		public Gender GenderFk { get; set; }
		
		public virtual long? CityId { get; set; }
		
        [ForeignKey("CityId")]
		public City CityFk { get; set; }
		
    }
}