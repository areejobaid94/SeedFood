using Infoseed.MessagingPortal.Sunshine.Models.Sunshine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Sunshine.Models
{
    public class App
    {
        public string id { get; set; }
    }

    public class Webhook
    {
        public string id { get; set; }
        public string version { get; set; }
    }

    public class Conversation
    {
        public string id { get; set; }
        public string type { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string externalId { get; set; }
    }

    public class Author
    {
        public string userId { get; set; }
        public string avatarUrl { get; set; }
        public string displayName { get; set; }
        public string type { get; set; }
        public User user { get; set; }
    }
  

    public class Content2
    {
        public string type { get; set; }
        public string text { get; set; }
        public string mediaUrl { get; set; }
        public string mediaType { get; set; }
        public double mediaSize { get; set; }
        public string altText { get; set; }
        public Coordinates coordinates { get; set; }
    }

    public class Source
    {
        public string integrationId { get; set; }
        public string originalMessageId { get; set; }
        public DateTime originalMessageTimestamp { get; set; }
        public string type { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public DateTime received { get; set; }
        public Author author { get; set; }
        public Content2 content { get; set; }
        public Source source { get; set; }
    }

    public class Payload
    {
        public Conversation conversation { get; set; }
        public Message message { get; set; }
    }

    public class Event
    {
        public string id { get; set; }
        public DateTime createdAt { get; set; }
        public string type { get; set; }
        public Payload payload { get; set; }
    }

    public class SunshineMsgReceivedModel
    {
        public App app { get; set; }
        public Webhook webhook { get; set; }
        public List<Event> events { get; set; }
    }


    public static class SmoochContentTypeEnum
    {

        public const string Text = "text";
        public const string Image = "image";
        public const string File = "file";
        public const string Location = "location";
    }

}
