using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Evaluations.Dtos
{
   public  class GetAllEvaluationsInput : PagedAndSortedResultRequestDto
    {

        public string Filter { get; set; }

        public string NameFilter { get; set; }
    }
}
