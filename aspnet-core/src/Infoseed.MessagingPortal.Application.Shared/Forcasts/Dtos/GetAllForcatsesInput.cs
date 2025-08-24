using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Forcasts.Dtos
{
    public class GetAllForcatsesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

   

        public string UserNameFilter { get; set; }

        public string CustomerNameFilter { get; set; }

        public string DealNameFilter { get; set; }

        public string ARRFilter { get; set; }

        public string OrderFeesFilter { get; set; }

        public DateTime? MaxCloseDateFilter { get; set; }
        public DateTime? MinCloseDateFilter { get; set; }

        public string DealAgeFilter { get; set; }

        public string DealStatusStatusFilter { get; set; }

        public string DealTypeDeal_TypeFilter { get; set; }

        public string TotalCommitFilter { get; set; }

        public string TotalClosedFilter { get; set; }

    }
}