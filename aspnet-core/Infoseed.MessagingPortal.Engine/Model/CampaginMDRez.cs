using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Infoseed.MessagingPortal.Engine.Model
{
    [BsonIgnoreExtraElements]
    public class CampaginMDRez
    {
        [BsonId]
        public ObjectId Id { get; set; } // Maps to the "_id" field

        public int tenantId { get; set; }
        public string campaignId { get; set; }
        public string phoneNumber { get; set; }
        public string messageId { get; set; }
        public string status { get; set; }
        public int statusCode { get; set; }
        public string failedDetails { get; set; }
        public bool is_accepted { get; set; }
        public bool is_delivered { get; set; }
        public bool is_read { get; set; }
        public bool is_sent { get; set; }
        public string delivered_detailsJson { get; set; }
        public string read_detailsJson { get; set; }
        public string sent_detailsJson { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
