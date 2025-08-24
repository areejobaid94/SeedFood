using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Areas.Dtos
{
    public class GetAllAreasForExcelInput
    {
		public string Filter { get; set; }

		public string AreaNameFilter { get; set; }

		public string AreaCoordinateFilter { get; set; }



    }
}