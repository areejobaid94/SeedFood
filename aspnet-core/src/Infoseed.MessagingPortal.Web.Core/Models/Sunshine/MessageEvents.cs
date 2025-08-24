using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Sunshine
{
    public class MessageEvents
    {
        
            public string trigger { get; set; }
            public App app { get; set; }
            public Message[] messages { get; set; }
            public Appuser appUser { get; set; }
            public Client client { get; set; }
            public Conversation conversation { get; set; }
            public string version { get; set; }
        

        public class App
        {
            public string _id { get; set; }
        }

        public class Appuser
        {
            public string _id { get; set; }
            public string userId { get; set; }
            public bool conversationStarted { get; set; }
            public string email { get; set; }
            public string surname { get; set; }
            public string givenName { get; set; }
            public DateTime signedUpAt { get; set; }
            public Properties properties { get; set; }
        }

        public class Properties
        {
            public string favoriteFood { get; set; }
        }

        public class Client
        {
            public string _id { get; set; }
            public DateTime lastSeen { get; set; }
            public string platform { get; set; }
            public string id { get; set; }
            public Info info { get; set; }
            public Raw raw { get; set; }
            public bool active { get; set; }
            public bool primary { get; set; }
            public string integrationId { get; set; }
        }

        public class Info
        {
            public string currentTitle { get; set; }
            public string currentUrl { get; set; }
            public string browserLanguage { get; set; }
            public string referrer { get; set; }
            public string userAgent { get; set; }
            public string URL { get; set; }
            public string sdkVersion { get; set; }
            public string vendor { get; set; }
        }

        public class Raw
        {
            public string currentTitle { get; set; }
            public string currentUrl { get; set; }
            public string browserLanguage { get; set; }
            public string referrer { get; set; }
            public string userAgent { get; set; }
            public string URL { get; set; }
            public string sdkVersion { get; set; }
            public string vendor { get; set; }
        }

        public class Conversation
        {
            public string _id { get; set; }
        }

        public class Message
        {
            public string _id { get; set; }
            public string type { get; set; }
            public string text { get; set; }
            public string role { get; set; }
            public string authorId { get; set; }
            public string name { get; set; }
            public float received { get; set; }
            public Source source { get; set; }
        }

        public class Source
        {
            public string type { get; set; }
        }

    }
}
