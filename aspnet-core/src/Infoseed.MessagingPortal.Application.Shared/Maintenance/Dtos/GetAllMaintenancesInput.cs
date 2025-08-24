using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Maintenance.Dtos
{
    public class GetAllMaintenancesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
