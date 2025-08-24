using Infoseed.MessagingPortal.SealingReuest.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.BotModel
{
    public class UpdateSaleOfferModel
    {

            public string ContactName { get; set; }
            public int ContactId { get; set; }
            public int TenantID { get; set; }
            public string PhoneNumber { get; set; }
            public decimal? Price { get; set; }
            public string information { get; set; }
            public AttachmentModel[] AttachmetArray { get; set; }
            public AttachmentModel[] AttachmetArrayTow { get; set; }
            public string ContactInformation { get; set; }
            public bool IsRequestForm { get; set; } = false;
            public SellingRequestFormModel RequestForm { get; set; } 
    }
}
