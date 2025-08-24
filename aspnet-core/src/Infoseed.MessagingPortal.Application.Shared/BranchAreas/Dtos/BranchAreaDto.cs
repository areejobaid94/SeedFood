
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.BranchAreas.Dtos
{
    public class BranchAreaDto : EntityDto<long>
    {
		public DateTime CreationTime { get; set; }


		 public long AreaId { get; set; }

		 		 public long BranchId { get; set; }

		 
    }
}