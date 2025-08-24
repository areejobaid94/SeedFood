using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Forcasts.Dtos
{
    public class GetAllForcatsesForExcelInput
    {
        public string Filter { get; set; }

        public string UserNameFilter { get; set; }

        public string TotalCommitFilter { get; set; }

        public string TotalClosedFilter { get; set; }

        public DateTime? MaxSubmitDateFilter { get; set; }
        public DateTime? MinSubmitDateFilter { get; set; }

    }
}