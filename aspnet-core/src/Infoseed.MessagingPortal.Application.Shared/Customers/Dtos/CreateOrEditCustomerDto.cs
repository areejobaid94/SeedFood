
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Customers.Dtos
{
    public class CreateOrEditCustomerDto : EntityDto<long?>
    {

		[Required]
		[StringLength(CustomerConsts.MaxCustomerNameLength, MinimumLength = CustomerConsts.MinCustomerNameLength)]
		public string CustomerName { get; set; }
		
		
		[StringLength(CustomerConsts.MaxPhoneNumberLength, MinimumLength = CustomerConsts.MinPhoneNumberLength)]
		public string PhoneNumber { get; set; }
		
		
		public string CustomerAddress { get; set; }
		
		
		[Required]
		public DateTime CreationTime { get; set; }
		
		
		public DateTime? DeletionTime { get; set; }
		
		
		[Required]
		[StringLength(CustomerConsts.MaxEmailAddressLength, MinimumLength = CustomerConsts.MinEmailAddressLength)]
		public string EmailAddress { get; set; }
		
		
		[Required]
		public bool IsActive { get; set; }
		
		
		[Required]
		public bool IsDeleted { get; set; }
		
		
		 public long? GenderId { get; set; }
		 
		 		 public long? CityId { get; set; }
		 
		 
    }
}