using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Loyalty.Dto
{
    public class LoyaltyLog
    {
        public long Id { get; set; }
        public string LoyaltyDefinitionJson { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public int? TenantId { get; set; }
        public long LoyaltyDefinitionId { get; set; }
        
    }
}
