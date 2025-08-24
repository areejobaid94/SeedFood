using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.OrderOffers
{
    [Table("OrderOffer")]
    public class OrderOffer : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get ; set; }
        public bool isAvailable { get; set; }
        public bool isPersentageDiscount { get; set; }
        public DateTime OrderOfferStart { get; set; }
        public DateTime OrderOfferEnd { get; set; }



        public DateTime OrderOfferDateStart { get; set; }
        public DateTime OrderOfferDateEnd { get; set; }
        public string OrderOfferDateStartS { get; set; }
        public string OrderOfferDateEndS { get; set; }


        public string OrderOfferStartS { get; set; }
        public string OrderOfferEndS { get; set; }
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
