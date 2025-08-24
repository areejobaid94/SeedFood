
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.BranchAreas.Dtos
{
    public class CreateOrEditBranchAreaDto : EntityDto<long?>
    {

		[Required]
		public DateTime CreationTime { get; set; }
		
		
		 public long AreaId { get; set; }
		 
		 		 public long BranchId { get; set; }
		 
		 
    }
}