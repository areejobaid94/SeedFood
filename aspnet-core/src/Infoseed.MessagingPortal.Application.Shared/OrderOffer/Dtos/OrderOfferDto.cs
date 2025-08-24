using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.OrderOffer.Dtos
{
    public class OrderOfferDto : EntityDto<long>
    {
        public bool isAvailable { get; set; }
        public bool isPersentageDiscount { get; set; }
        public DateTime OrderOfferStart { get; set; } = DateTime.Now;
        public DateTime OrderOfferEnd { get; set; } = DateTime.Now;
        public string OrderOfferStartS { get; set; }
        public string OrderOfferEndS { get; set; }

        public dynamic[,] SelectetDate { get; set; }

        public DateTime OrderOfferDateStart { get; set; } = DateTime.Now;
        public DateTime OrderOfferDateEnd { get; set; } = DateTime.Now;
        public string OrderOfferDateStartS { get; set; }
        public string OrderOfferDateEndS { get; set; }


        public string Cities { get; set; }
        public string Area { get; set; }

        public decimal FeesStart { get; set; }
        public decimal FeesEnd { get; set; }

        public decimal NewFees { get; set; }


        public bool isBranchDiscount { get; set; }
        public string BranchesName { get; set; }
        public string BranchesIds { get; set; }
    }
}
