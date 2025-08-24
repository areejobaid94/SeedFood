using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.MultiTenancy.Dto
{
    public class TenantsToExcelDto
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? TotalFreeConversation { get; set; }
        public decimal? RemainingFreeConversation { get; set; }
        public decimal? TotalUIConversation { get; set; }
        public decimal? RemainingUIConversation { get; set; }
        public decimal? TotalBIConversation { get; set; }
        public decimal? RemainingBIConversation { get; set; }


    }
}
