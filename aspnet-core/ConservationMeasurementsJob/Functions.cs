using ConservationMeasurementsJob.DAL;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebJobEntities;

namespace ConservationMeasurementsJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("conservation-measurements")] string message, TextWriter log)
        {
            //log.WriteLine(message);
            ConservationMeasurementMessage obj = JsonConvert.DeserializeObject<ConservationMeasurementMessage>(message);
           

            ConversationRepository.LogConversationMasurements(new Entities.ConservationMeasurement()
            {
                CommunicationInitiated = obj.CommunicationInitiated,
                TenantId = obj.TenantId,
                ConversationId = obj.ConversationId,
                MessageDateTime = obj.MessageDateTime,
                PhoneNumber= obj.PhoneNumber,
                MessageId= obj.MessageId,
                MessageStatusId = obj.MessageStatusId

            },log);
        }
    }
}
