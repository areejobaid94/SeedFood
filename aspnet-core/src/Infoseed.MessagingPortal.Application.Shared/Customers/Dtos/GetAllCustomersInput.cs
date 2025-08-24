using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Customers.Dtos
{
    public class GetAllCustomersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string CustomerNameFilter { get; set; }

		public string PhoneNumberFilter { get; set; }

		public string CustomerAddressFilter { get; set; }

		public DateTime? MaxCreationTimeFilter { get; set; }
		public DateTime? MinCreationTimeFilter { get; set; }

		public DateTime? MaxDeletionTimeFilter { get; set; }
		public DateTime? MinDeletionTimeFilter { get; set; }

		public string EmailAddressFilter { get; set; }

		public int? IsActiveFilter { get; set; }

		public int? IsDeletedFilter { get; set; }


		 public string GenderNameFilter { get; set; }

		 		 public string CityNameFilter { get; set; }

		 
    }
}