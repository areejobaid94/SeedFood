using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Banks.Dtos
{
    public class GetAllBanksInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

    }
}