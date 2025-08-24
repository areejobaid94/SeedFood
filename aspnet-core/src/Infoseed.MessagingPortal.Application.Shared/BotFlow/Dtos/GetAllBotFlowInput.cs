using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.BotFlow.Dtos
{
    public class GetAllBotFlowInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
