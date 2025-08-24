
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Branches.Dtos
{
    public class BranchDto : EntityDto<long>
    {
		public string Name { get; set; }

        public decimal? DeliveryCost { get; set; }

        public  string RestaurantName { get; set; }

    }
}