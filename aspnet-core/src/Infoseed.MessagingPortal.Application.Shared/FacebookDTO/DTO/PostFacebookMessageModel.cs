using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.FacebookDTO.DTO
{
    public class PostFacebookMessageModel
    {
        public string MediaUrl { get; set; }
        public string TypeContent { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public string RecipientId { get; set; } // Facebook uses recipient ID instead of 'to'
        public string MessagingType { get; set; } // e.g., "RESPONSE", "UPDATE", "MESSAGE_TAG"
        public string NotificationType { get; set; } // e.g., "REGULAR", "SILENT_PUSH", "NO_PUSH"
        public string Tag { get; set; } // Optional, for message tags
        public string Type { get; set; }

        public TextMessage Text { get; set; }
        public ImageAttachment Image { get; set; }
        public AudioAttachment Audio { get; set; }
        public FileAttachment File { get; set; }
        public VideoAttachment Video { get; set; }
        public LocationAttachment Location { get; set; }
        public TemplateAttachment Template { get; set; }
        public QuickReply QuickReplyData { get; set; } // Property for quick replies
        public Postback PostbackData { get; set; } // Property for postback

        public class TextMessage
        {
            public string Text { get; set; }
        }

        public class ImageAttachment
        {
            public string Url { get; set; }
            public bool Reusable { get; set; } // Indicates if the attachment is reusable
        }

        public class AudioAttachment
        {
            public string Url { get; set; }
            public bool Reusable { get; set; }
        }

        public class FileAttachment
        {
            public string Url { get; set; }
            public bool Reusable { get; set; }
        }

        public class VideoAttachment
        {
            public string Url { get; set; }
            public bool Reusable { get; set; }
        }

        public class LocationAttachment
        {
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
        }

        public class TemplateAttachment
        {
            public string Payload { get; set; } 
        }

        public class QuickReply
        {
            public List<QuickReplyItem> QuickReplies { get; set; } 
        }

        public class QuickReplyItem
        {
            public string ContentType { get; set; } 
            public string Title { get; set; }      
            public string Payload { get; set; }    
            public string ImageUrl { get; set; }   
        }

        public class Postback
        {
            public string Title { get; set; }
            public string Payload { get; set; }
        }
    }
}
