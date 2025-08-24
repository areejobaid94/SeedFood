using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebJobEntities;

namespace ConservationMeasurementsJob.Entities
{
   public class ConservationMeasurement
    {
        public int TenantId { get; set; }
        public CommunicationInitiated CommunicationInitiated { get; set; }
        public DateTime MessageDateTime { get; set; }
        public string ConversationId { get; set; }

        public string PhoneNumber { get; set; }
        public string MessageId { get; set; }
        public int MessageStatusId { get; set; }
        
    }
}
