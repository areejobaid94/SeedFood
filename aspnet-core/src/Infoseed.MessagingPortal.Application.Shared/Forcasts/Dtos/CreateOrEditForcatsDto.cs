using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Forcasts.Dtos
{
    public class CreateOrEditForcatsDto : EntityDto<int?>
    {

        public string UserName { get; set; }

        public string CustomerName { get; set; }

        public string DealName { get; set; }

        public string ARR { get; set; }

        public string OrderFees { get; set; }

        public DateTime CloseDate { get; set; }

        public string DealAge { get; set; }

        public int DealStatusId { get; set; }

        public int DealTypeId { get; set; }

        public string TotalCommit { get; set; }

        public string TotalClosed { get; set; }

        public DateTime SubmitDate { get; set; }

    }
}