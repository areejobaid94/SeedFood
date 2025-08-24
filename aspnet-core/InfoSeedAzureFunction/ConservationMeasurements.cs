using InfoSeedAzureFunction.DAL;
using InfoSeedAzureFunction.Model;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InfoSeedAzureFunction
{
    public class ConservationMeasurements
    {
        [FunctionName("ConservationMeasurementsFunction")]
        public static void ProcessQueueMessage([QueueTrigger("conservation-measurements", Connection = "AzureWebJobsStorage")] string message, TextWriter log)
        {
            // log.WriteLine(message); [QueueTrigger("conservation-measurements", Connection = "AzureWebJobsStorage")] string message
            ConservationMeasurementMessage obj = JsonConvert.DeserializeObject<ConservationMeasurementMessage>(message);


            ConversationRepository.LogConversationMasurements(new Entities.ConservationMeasurement()
            {
                CommunicationInitiated = obj.CommunicationInitiated,
                TenantId = obj.TenantId,
                ConversationId = obj.ConversationId,
                MessageDateTime = obj.MessageDateTime,
                PhoneNumber = obj.PhoneNumber,
                MessageId = obj.MessageId,
                MessageStatusId = obj.MessageStatusId,
                creation_timestamp = obj.creation_timestamp,
                expiration_timestamp = obj.expiration_timestamp

            }, log);
        }
    }
}
