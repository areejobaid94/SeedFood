using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class BundleModel
    {
        public int TenantId { get; set; }

        public decimal TotalConversation { get; set; }

        //Free Conversations
        public decimal TotalFreeConversation { get; set; }
        public decimal TotalFacebookEntry { get; set; }
        public decimal TotalFreeTier { get; set; }

        //Paid Conversations
        public decimal TotalPaidConversation { get; set; }
        public decimal TotalMarketingConversation { get; set; }
        public decimal TotalUtilityConversation { get; set; }
        public decimal TotalServicesConversation { get; set; }

        //Total The approximate charge
        public float TotalCharge { get; set; }
        public float TotalMarketingCharge { get; set; }
        public float TotalUtilityCharge { get; set; }
        public float TotalServicesCharge { get; set; }

    }
}
