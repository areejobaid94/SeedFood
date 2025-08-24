using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Instgram.InstgramDTO
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    namespace Instagram.Webhook.Models
    {
        public class InstagramWebhookModel
        {
            [JsonProperty("object")]
            public string Object { get; set; }

            [JsonProperty("entry")]
            public List<InstagramEntry> Entry { get; set; }
        }

        public class InstagramEntry
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("time")]
            public long Time { get; set; }

            [JsonProperty("changes")]
            public List<InstagramChange> Changes { get; set; }

            [JsonProperty("messaging")]
            public List<InstagramMessaging> Messaging { get; set; }
        }

        public class InstagramChange
        {
            [JsonProperty("field")]
            public string Field { get; set; } 

            [JsonProperty("value")]
            public InstagramChangeValue Value { get; set; }
        }

        public class InstagramChangeValue
        {
            [JsonProperty("from")]
            public InstagramUser From { get; set; }

            [JsonProperty("sender")]
            public InstagramUser Sender { get; set; }

            [JsonProperty("recipient")]
            public InstagramUser Recipient { get; set; }

            [JsonProperty("timestamp")]
            public long? Timestamp { get; set; }

            // Comment fields
            [JsonProperty("media")]
            public InstagramMedia Media { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("parent_id")]
            public string ParentId { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            // Message reaction fields
            [JsonProperty("reaction")]
            public InstagramReaction Reaction { get; set; }

            // Message fields
            [JsonProperty("message")]
            public InstagramMessage Message { get; set; }

            // Handover fields
            [JsonProperty("pass_thread_control")]
            public InstagramPassThreadControl PassThreadControl { get; set; }

            // Optin fields
            [JsonProperty("optin")]
            public InstagramOptin Optin { get; set; }

            // Postback fields
            [JsonProperty("postback")]
            public InstagramPostback Postback { get; set; }

            // Referral fields
            [JsonProperty("referral")]
            public InstagramReferral Referral { get; set; }

            // Seen fields
            [JsonProperty("read")]
            public InstagramRead Read { get; set; }

            // Standby field
            [JsonProperty("standby")]
            public string Standby { get; set; }
        }

        public class InstagramUser
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }
        }

        public class InstagramMedia
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("media_product_type")]
            public string MediaProductType { get; set; } 
        }

        public class InstagramReaction
        {
            [JsonProperty("mid")]
            public string Mid { get; set; }

            [JsonProperty("action")]
            public string Action { get; set; }

            [JsonProperty("reaction")]
            public string Reaction { get; set; }

            [JsonProperty("emoji")]
            public string Emoji { get; set; }
        }

        public class InstagramMessage
        {
            [JsonProperty("mid")]
            public string Mid { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("attachments")]
            public List<InstagramAttachment> Attachments { get; set; }
        }

        public class InstagramAttachment
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("payload")]
            public InstagramAttachmentPayload Payload { get; set; }
        }

        public class InstagramAttachmentPayload
        {
            [JsonProperty("url")]
            public string Url { get; set; }
        }

        public class InstagramPassThreadControl
        {
            [JsonProperty("previous_owner_app_id")]
            public string PreviousOwnerAppId { get; set; }

            [JsonProperty("new_owner_app_id")]
            public string NewOwnerAppId { get; set; }

            [JsonProperty("metadata")]
            public string Metadata { get; set; }
        }

        public class InstagramOptin
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("payload")]
            public string Payload { get; set; }

            [JsonProperty("notification_messages_token")]
            public string NotificationMessagesToken { get; set; }

            [JsonProperty("notification_messages_frequency")]
            public string NotificationMessagesFrequency { get; set; }

            [JsonProperty("token_expiry_timestamp")]
            public long TokenExpiryTimestamp { get; set; }

            [JsonProperty("user_token_status")]
            public string UserTokenStatus { get; set; }

            [JsonProperty("notification_messages_timezone")]
            public string NotificationMessagesTimezone { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }
        }

        public class InstagramPostback
        {
            [JsonProperty("mid")]
            public string Mid { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("payload")]
            public string Payload { get; set; }
        }

        public class InstagramReferral
        {
            [JsonProperty("ref")]
            public string Ref { get; set; }

            [JsonProperty("source")]
            public string Source { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class InstagramRead
        {
            [JsonProperty("mid")]
            public string Mid { get; set; }
        }

        public class InstagramMessaging
        {
            [JsonProperty("sender")]
            public InstagramUser Sender { get; set; }

            [JsonProperty("recipient")]
            public InstagramUser Recipient { get; set; }

            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("message")]
            public InstagramMessage Message { get; set; }

            [JsonProperty("reaction")]
            public InstagramReaction Reaction { get; set; }

            [JsonProperty("pass_thread_control")]
            public InstagramPassThreadControl PassThreadControl { get; set; }

            [JsonProperty("optin")]
            public InstagramOptin Optin { get; set; }

            [JsonProperty("postback")]
            public InstagramPostback Postback { get; set; }

            [JsonProperty("referral")]
            public InstagramReferral Referral { get; set; }

            [JsonProperty("read")]
            public InstagramRead Read { get; set; }
        }
    }
}
