
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Customers.Dtos
{
    public class CustomerDto : EntityDto<long>
    {
		public string CustomerName { get; set; }

		public string PhoneNumber { get; set; }

		public string CustomerAddress { get; set; }

		public DateTime CreationTime { get; set; }

		public DateTime? DeletionTime { get; set; }

		public string EmailAddress { get; set; }

		public bool IsActive { get; set; }

		public bool IsDeleted { get; set; }


		 public long? GenderId { get; set; }

		 		 public long? CityId { get; set; }

		 
    }
}