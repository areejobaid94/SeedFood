using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.FacebookDTO.DTO
{
    public class FacebookContent
    {
        public string Type { get; set; }          // Type of message (e.g., "text", "image", "video")
        public string Text { get; set; }          // Text content of the message
        public string MediaUrl { get; set; }       // URL of the media (image, video, file, etc.)
        public string AltText { get; set; }       // Alternative text for media (if applicable)
        public string AgentName { get; set; }      // Name of the agent (if applicable)
        public string AgentId { get; set; }       // ID of the agent (if applicable)
        public string FileName { get; set; }       // Name of the file (if applicable)
        public string UserId { get; set; }         // ID of the user (recipient)
        public string ConversationId { get; set; } // ID of the conversation
        public int TenantId { get; set; }          // Tenant ID (if applicable)
        public bool IsQuickReply { get; set; }     // Indicates if the message contains quick replies
        public List<QuickReply> QuickReplies { get; set; } // List of quick replies (if applicable)
        public List<string> Buttons { get; set; }
        public bool IsButton { get; set; }

        public class QuickReply
        {
            public string ContentType { get; set; } // Type of quick reply (e.g., "text", "location")
            public string Title { get; set; }      // Title of the quick reply
            public string Payload { get; set; }     // Payload of the quick reply
            public string ImageUrl { get; set; }   // URL of the image (if applicable)
        }
    }
}
