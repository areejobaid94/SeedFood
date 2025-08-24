using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Sunshine
{
    public class SunshineMsgReceivedV1Model
    {

       
            public string trigger { get; set; }
            public string version { get; set; }
            public App app { get; set; }
            public Appuser appUser { get; set; }
            public Conversation conversation { get; set; }
            public Message[] messages { get; set; }
        
        public class App
        {
            public string _id { get; set; }
        }

        public class Appuser
        {
            public string surname { get; set; }
            public string givenName { get; set; }
            public DateTime signedUpAt { get; set; }
            public bool hasPaymentInfo { get; set; }
            public string _id { get; set; }
            public bool conversationStarted { get; set; }
            public Client[] clients { get; set; }
            public object[] pendingClients { get; set; }
            public Device[] devices { get; set; }
            public bool credentialRequired { get; set; }
            public Properties properties { get; set; }
        }

        public class Properties
        {
        }

        public class Client
        {
            public string integrationId { get; set; }
            public string externalId { get; set; }
            public string id { get; set; }
            public string displayName { get; set; }
            public string status { get; set; }
            public Raw raw { get; set; }
            public DateTime lastSeen { get; set; }
            public DateTime linkedAt { get; set; }
            public string _id { get; set; }
            public string platform { get; set; }
            public bool active { get; set; }
            public bool blocked { get; set; }
            public bool primary { get; set; }
        }

        public class Raw
        {
            public Profile profile { get; set; }
            public string from { get; set; }
        }

        public class Profile
        {
            public string name { get; set; }
        }

        public class Device
        {
            public string integrationId { get; set; }
            public string externalId { get; set; }
            public string id { get; set; }
            public string displayName { get; set; }
            public string status { get; set; }
            public Raw1 raw { get; set; }
            public DateTime lastSeen { get; set; }
            public DateTime linkedAt { get; set; }
            public string _id { get; set; }
            public string platform { get; set; }
            public bool active { get; set; }
            public bool blocked { get; set; }
            public bool primary { get; set; }
        }

        public class Raw1
        {
            public Profile1 profile { get; set; }
            public string from { get; set; }
        }

        public class Profile1
        {
            public string name { get; set; }
        }

        public class Conversation
        {
            public string _id { get; set; }
        }

        public class Message
        {
            public string to { get; set; }
            public string from { get; set; }
            public string id { get; set; }
            public Text text { get; set; }
          //  public Image image { get; set; }
            public Video video { get; set; }
           // public Location location { get; set; }
            public Document document { get; set; }
            public Voice voice { get; set; }
            public string timestamp { get; set; }
            public string type { get; set; }
            public string textD360 { get; set; }
            public string mediaUrl { get; set; }
            public string typeD360 { get; set; }


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
            public string caption { get; set; }
            public string link { get; set; }
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
            public string link { get; set; }
        }
        public class Voice
        {
            public string id { get; set; }
            public string mime_type { get; set; }
            public string sha256 { get; set; }
            public string caption { get; set; }
            public string link { get; set; }
        }

        public class Source
        {
            public string type { get; set; }
            public string id { get; set; }
            public string integrationId { get; set; }
            public string originalMessageId { get; set; }
            public int originalMessageTimestamp { get; set; }
        }

    }
}
