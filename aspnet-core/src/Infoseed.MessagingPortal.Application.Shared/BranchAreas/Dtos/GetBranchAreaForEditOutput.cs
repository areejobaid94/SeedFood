using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.BranchAreas.Dtos
{
    public class GetBranchAreaForEditOutput
    {
		public CreateOrEditBranchAreaDto BranchArea { get; set; }

		public string AreaAreaName { get; set;}

		public string BranchName { get; set;}


    }
}