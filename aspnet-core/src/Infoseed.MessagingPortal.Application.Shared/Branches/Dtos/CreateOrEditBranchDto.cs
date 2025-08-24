
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Branches.Dtos
{
    public class CreateOrEditBranchDto : EntityDto<long?>
    {

		[Required]
		public string Name { get; set; }

		public string RestaurantName { get; set; }
		public decimal DeliveryCost { get; set; }

	}
}