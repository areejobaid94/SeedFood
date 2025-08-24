using Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Sunshine
{
    public class CustomerChat
    {
        public DateTime? lastNotificationsData { get; set; } = DateTime.UtcNow;
        public string notificationsText { get; set; }
        public string notificationID { get; set; }
        public int UnreadMessagesCount { get; set; }

        public int? TenantId { get; set; }
        public string messageId { get; set; }
        public string userId { get; set; }
        public string SunshineConversationId { get; set; }
        public DateTime? CreateDate { get; set; }=DateTime.UtcNow;
        public string type { get; set; }
        public string text { get; set; }
        public int status { get; set; }
        public string fileName { get; set; }
        public MessageSenderType sender { get; set; }
        public string mediaUrl { get; set; }
        public string agentName { get; set; }
        public string agentId { get; set; }
        public InfoSeedContainerItemTypes ItemType { get; set; } = InfoSeedContainerItemTypes.ConversationItem;

        public bool IsButton { get; set; }

        public List<string> Buttons { get; set; }
       // public ReferralMassages referral { get; set; }


        public string source_url { get; set; }
        public string source_id { get; set; }
        public string source_type { get; set; }
        public string headline { get; set; }
        public string body { get; set; }
        public string media_type { get; set; }
        public string image_url { get; set; }
        public string video_url { get; set; }
        public string thumbnail_url { get; set; }



        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public int _ts { get; set; }
        public bool IsQuickReply { get; internal set; }
        public List<QuickReply> QuickReplies { get; set; }

    }
    public class QuickReply
    {
        public string ContentType { get; set; } 
        public string Title { get; set; }      
        public string Payload { get; set; }    
        public string ImageUrl { get; set; }  
    }

public enum MessageSenderType
    {
        Customer=1,
        TeamInbox=2
    }

    public  enum Messagestatus 
    {
      New=1,
      Read=2,
    }

    //public class ReferralMassages
    //{
    //    public string source_url { get; set; }
    //    public string source_id { get; set; }
    //    public string source_type { get; set; }
    //    public string headline { get; set; }
    //    public string body { get; set; }
    //    public string media_type { get; set; }
    //    public string image_url { get; set; }
    //    public string video_url { get; set; }
    //    public string thumbnail_url { get; set; }
    //}
}
