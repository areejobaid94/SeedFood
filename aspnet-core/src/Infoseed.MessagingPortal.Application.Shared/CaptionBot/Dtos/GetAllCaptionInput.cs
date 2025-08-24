using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.CaptionBot.Dtos
{
    public class GetAllCaptionInput : PagedAndSortedResultRequestDto
    {
        public string NameFilter { get; set; }
    }
}
