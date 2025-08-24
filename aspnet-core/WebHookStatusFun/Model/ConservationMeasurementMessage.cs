using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
    public class ConservationMeasurementMessage
    {
        public int TenantId { get; set; }
        public CommunicationInitiated CommunicationInitiated { get; set; }

        public DateTime MessageDateTime { get; set; }

        public string ConversationId { get; set; }

        public string PhoneNumber { get; set; }
        public string MessageId { get; set; }
        public int MessageStatusId { get; set; }

        public int creation_timestamp { get; set; }
        public int expiration_timestamp { get; set; }
    }
}
