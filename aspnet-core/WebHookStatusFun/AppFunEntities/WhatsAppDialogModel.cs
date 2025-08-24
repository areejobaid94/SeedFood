using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHookStatusFun
{
    public class WhatsAppDialogModel
    {
        public Message[] messages { get; set; }
        public Contact[] contacts { get; set; }
        public Status[] statuses { get; set; }
        public class Image
        {
            public string id { get; set; }
            public string mime_type { get; set; }
            public string sha256 { get; set; }
        }

        public class Contact
        {
            public Profile profile { get; set; }
            public string wa_id { get; set; }
        }

        public class Profile
        {
            public string name { get; set; }
        }

        public class Status
        {
            public Conversation conversation { get; set; }
            public string id { get; set; }
            public Pricing pricing { get; set; }
            public string recipient_id { get; set; }
            public string status { get; set; }
            public string timestamp { get; set; }
        }

        public class Conversation
        {
            public string id { get; set; }
        }

        public class Pricing
        {
            public bool billable { get; set; }
            public string pricing_model { get; set; }
        }

        public class Message
        {
            public string to { get; set; }
            public string from { get; set; }
            public string id { get; set; }
            public Text text { get; set; }
            public Image image { get; set; }
            public Video video { get; set; }
            public Location location { get; set; }
            public Document document { get; set; }
            public Voice voice { get; set; }
            public string timestamp { get; set; }
            public string type { get; set; }

        }
        public class Text
        {
            public string body { get; set; }
        }

        public class Video
        {
            public string id { get; set; }
            public string mime_type { get; set; }
            public string sha256 { get; set; }
        }
        public class Location
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
        }
        public class Document
        {
            public string caption { get; set; }
            public string filename { get; set; }
            public string id { get; set; }
            public string mime_type { get; set; }
            public string sha256 { get; set; }
        }
        public class Voice
        {
            public string id { get; set; }
            public string mime_type { get; set; }
            public string sha256 { get; set; }
        }



    }
}
