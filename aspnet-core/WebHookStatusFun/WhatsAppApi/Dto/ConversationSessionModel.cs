using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
    public class ConversationSessionModel
    {
        public long? Id { get; set; }
        public string ConversationId { get; set; }
        public DateTime? ConversationDateTime { get; set; }
        public string InitiatedBy { get; set; }
        public string PhoneNumber { get; set; }
        public int? TenantId { get; set; }
        public int? ContactId { get; set; }
    }
}
