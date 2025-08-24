using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ExtraOrderDetails.Dtos
{
    public class GetAllExtraOrderDetailsInput : PagedAndSortedResultRequestDto
    {
        public string NameFilter { get; set; }
    }
}
