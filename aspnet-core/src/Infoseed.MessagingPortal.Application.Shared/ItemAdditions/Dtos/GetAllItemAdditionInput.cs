using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAdditions.Dtos
{
    public class GetAllItemAdditionInput : PagedAndSortedResultRequestDto
    {
        public string NameFilter { get; set; }
    }
}
