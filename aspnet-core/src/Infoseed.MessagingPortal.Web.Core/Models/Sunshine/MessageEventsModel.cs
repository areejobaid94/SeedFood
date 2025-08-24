using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Sunshine
{
    public class MessageEventsModel
    {

        
            public App app { get; set; }
            public Webhook webhook { get; set; }
            public Event[] events { get; set; }
       

        public class App
        {
            public string id { get; set; }
        }

        public class Webhook
        {
            public string id { get; set; }
            public string version { get; set; }
        }

        public class Event
        {
            public string id { get; set; }
            public DateTime createdAt { get; set; }
            public string type { get; set; }
            public Payload payload { get; set; }
        }

        public class Payload
        {
            public Conversation conversation { get; set; }
            public Message message { get; set; }
        }

        public class Conversation
        {
            public string id { get; set; }
            public string type { get; set; }
        }

        public class Message
        {
            public string id { get; set; }
            public DateTime received { get; set; }
            public Author author { get; set; }
            public Content content { get; set; }
            public Source source { get; set; }
            public Quotedmessage quotedMessage { get; set; }
        }

        public class Author
        {
            public string userId { get; set; }
            public string avatarUrl { get; set; }
            public string displayName { get; set; }
            public string type { get; set; }
            public User user { get; set; }
        }

        public class User
        {
            public string id { get; set; }
            public string externalId { get; set; }
        }

        public class Content
        {
            public string type { get; set; }
            public string text { get; set; }
        }

        public class Source
        {
            public string type { get; set; }
            public string integrationId { get; set; }
            public string originalMessageId { get; set; }
            public DateTime originalMessageTimestamp { get; set; }
        }

        public class Quotedmessage
        {
            public string type { get; set; }
            public Message1 message { get; set; }
        }

        public class Message1
        {
            public string id { get; set; }
            public DateTime received { get; set; }
            public Author1 author { get; set; }
            public Content1 content { get; set; }
            public Source1 source { get; set; }
        }

        public class Author1
        {
            public string type { get; set; }
        }

        public class Content1
        {
            public string type { get; set; }
            public string mediaUrl { get; set; }
        }

        public class Source1
        {
            public string type { get; set; }
            public string integrationId { get; set; }
            public string originalMessageId { get; set; }
            public DateTime originalMessageTimestamp { get; set; }
        }

    }
}
