using Abp.Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class FacebookModel
    {
        public string Object { get; set; }
        public List<Entry> Entry { get; set; }
    }
    public class Entry
    {
        public string Id { get; set; }
        public long Time { get; set; }
        public List<Change> Changes { get; set; }
        public List<LeadgenChange> leadgenChange { get; set; }

        public List<Messaging> Messaging { get; set; }
    }
    public class LeadgenChange
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("value")]
        public Leadgen Value { get; set; }
    }
    public class Leadgen
    {
        [JsonProperty("ad_id")]
        public string AdId { get; set; }

        [JsonProperty("form_id")]
        public string FormId { get; set; }

        [JsonProperty("leadgen_id")]
        public string LeadgenId { get; set; }

        [JsonProperty("created_time")]
        public long CreatedTime { get; set; }

        [JsonProperty("page_id")]
        public string PageId { get; set; }

        [JsonProperty("adgroup_id")]
        public string AdgroupId { get; set; }
    }
    public class MessageContext
    {
        [JsonProperty("detections")]
        public List<Detection> Detections { get; set; }

        [JsonProperty("suggestions")]
        public List<Suggestion> Suggestions { get; set; }
    }
    public class Suggestion
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Mid { get; set; }
        public string EventName { get; set; }
        public string Model { get; set; }
    }
    public class Detection
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Mid { get; set; }
        public string EventName { get; set; }
        public string Model { get; set; }
    }
    public class Messaging
    {
        public Recipient Recipient { get; set; }
        public Sender Sender { get; set; }
        public string GroupId { get; set; }
        public MessageContext MessageContext { get; set; }

        public string CommentId { get; set; }
        public string PostId { get; set; }
        public long CreatedTime { get; set; }
        public string Item { get; set; }
        public string Verb { get; set; }
        public string MessageText { get; set; }
        public string Field { get; set; }
        public string ParentId { get; set; }
        public string timestamp { get; set; }
        public Delivery Delivery { get; set; }
        public Reaction Reaction { get; set; }
        public MessageContent MessageContent { get; set; }
        public MessageModel Message { get; set; }
        public Message_edit Message_edit { get; set; }
        public AccountLinking AccountLinking { get; set; }
        public Leadgen Leadgen { get; set; }
        public LiveVideos LiveVideos { get; set; }
        public Mention Mention { get; set; }
        public Read Read { get; set; }
        public Account_linking Account_linking { get; set; }
        public CustomerInformation Messaging_customer_information { get; set; }
        public Form Form { get; set; }
        public Optin Optin { get; set; }
        public Postback Postback { get; set; }
        public OrderFacebook Order { get; set; }



  
       

      



    }


    public class Attachment11
    {
        public string type { get; set; }
        public Payload11 payload { get; set; }
    }

    public class Payload11
    {
        public string url { get; set; }
    }


    public class Postback
    {
        public string Mid { get; set; }
        public string Title { get; set; }
        public string Payload { get; set; }
        public ReferralFacebook Referral { get; set; }

    }

    public class ReferralFacebook
    {
        public string Ref { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
    }
    public class Ads_context_data
    {
        public string Ad_title { get; set; }
        public string Photo_url { get; set; }
        public string Video_url { get; set; }
        public string Post_id { get; set; }
        public string roduct_id { get; set; }

    }
    public class policy_enforcement
    {
        public string Action;
        public string Reason;
    }
    public class Optin
    {
        public string Type { get; set; }
        public string Payload { get; set; }
        public string Notification_messages_token { get; set; }
        public string Notification_messages_frequency { get; set; }
        public long Token_expiry_timestamp { get; set; }
        public string User_token_status { get; set; }

    }

    public class Form
    {
        public string Id;
    }
    public class Messaging_customer_information
    {
        public string Status;
        public string Authorization_code;
    }
    public class CustomerInformation
    {
        public List<Screen> Screens { get; set; }
    }

    public class Response
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class Account_linking
    {
        public string Status;
        public string Authorization_code;
    }
    public class Read
    {
        public string watermark;
    }
    public class MessageModel
    {
        public string Is_echo { get; set; }
        public string Mid { get; set; }
        public string Text { get; set; }
        public Attachment11[] attachments { get; set; }

    }
    public class Recipient
    {
        public string Id { get; set; }
    }

    public class Delivery
    {
        public long Watermark { get; set; }
        public List<string> Mids { get; set; }
    }

    public class Reaction
    {
        public string Mid { get; set; }
        public string Action { get; set; }
        public string Emoji { get; set; }
        public string ReactionText { get; set; }
        public string reaction { get; set; }

    }

    public class MessageContent
    {
        public string Mid { get; set; }
        public string Text { get; set; }
        public List<Command> Commands { get; set; }
    }

    public class Command
    {
        public string Name { get; set; }
    }

    public class AccountLinking
    {
        public string Status { get; set; }
        public string AuthorizationCode { get; set; }
    }

    public class MessagingCustomerInformation
    {
        public List<Screen> Screens { get; set; }
    }

    public class Screen
    {
        public string ScreenId { get; set; }
        public List<ScreenResponse> Responses { get; set; }
    }

    public class ScreenResponse
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }


    public class LiveVideos
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class Mention
    {
        [JsonProperty("post_id")]
        public string PostId { get; set; }

        [JsonProperty("sender_name")]
        public string SenderName { get; set; }

        [JsonProperty("item")]
        public string Item { get; set; }

        [JsonProperty("sender_id")]
        public string SenderId { get; set; }

        [JsonProperty("verb")]
        public string Verb { get; set; }
    }



    public class Sender
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
    }
    public class Change
    {
        public string Field { get; set; }
        public string action { get; set; }
        public Label label { get; set; }

        [JsonConverter(typeof(ChangeValueConverter))]
        public ChangeValue Value { get; set; }
        [JsonExtensionData]
        public Dictionary<string, JToken> AdditionalProperties { get; set; }
        public Page Page { get; set; }
    }

    // This class represents a structured object when "value" is not a simple string
    public class ChangeValue
    {
        public User User { get; set; }
        [JsonProperty("page_id")]
        public long page_id { get; set; }
        public long page_about_story_id { get; set; }

        public string invoice_id { get; set; }
        public string external_invoice_id { get; set; }
        public string action { get; set; }
        public Label label { get; set; }
        public long InvoiceId { get; set; }
        public long PageId { get; set; }
        public string ExternalInvoiceId { get; set; }
        public Updates Updates { get; set; }
        public List<Payment> Payments { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
        [JsonProperty("item")]
        public string Item { get; set; }

        [JsonProperty("post_id")]
        public string PostId { get; set; }

        [JsonProperty("verb")]
        public string Verb { get; set; }

        [JsonProperty("published")]
        public string Published { get; set; }

        [JsonProperty("created_time")]
        public string CreatedTime { get; set; }

        [JsonProperty("group_id")]
        public string GroupId { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("parent_id")]
        public string ParentId { get; set; }

        [JsonProperty("comment_id")]
        public string CommentId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("from")]
        public FromUser From { get; set; }

        public Message_edit message_edit { get; set; }
        public Proposal Proposal { get; set; }
        public Page Page { get; set; }
        public string Effective_time { get; set; }
        public string Change_type { get; set; }
        public string Timer_status { get; set; }
        public string Product_item_id { get; set; }
        //public string Status { get; set; }
        public StatusFacebook Status { get; set; }

    }
    public class OrderFacebook
    {
        public List<Product> Products { get; set; }
        public string Note { get; set; }
        public string Source { get; set; }
        public string Source_id { get; set; }
    }

    public class Product
    {
        public long Id { get; set; }
        public string Retailer_id { get; set; }
        public string Name { get; set; }
        public decimal Unit_price { get; set; }
        public string Currency { get; set; }
        public long Quantity { get; set; }
    }
    public class Page
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class Proposal
    {
        public string Id { get; set; }
        public string Acceptance_status { get; set; }
        public string Category { get; set; }
    }
    public class Message_edit
    {
        public string Mid;
        public string Text;
    }

    public class Updates
    {
        public ShippingAddress ShippingAddress { get; set; }
        public PaymentsUpdate Payments { get; set; }
    }

    public class PaymentsUpdate
    {
        [JsonProperty("previous_value")]
        public List<Payment> PreviousValue { get; set; }

        [JsonProperty("new_value")]
        public List<Payment> NewValue { get; set; }
    }


    public class ShippingAddress
    {
        public AddressDetails PreviousValue { get; set; }
        public AddressDetails NewValue { get; set; }
    }

    public class AddressDetails
    {
        public ContactInfo ContactInfo { get; set; }
        public Address AddressDetail { get; set; }
    }
    public class ContactInfo
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class Address
    {
        public string Country { get; set; }
        public string AddressLine1 { get; set; }
    }
    public class Payment
    {
        public PaymentAmount PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public long CreationTime { get; set; }
        public Metadata Metadata { get; set; }
    }
    public class PaymentAmount
    {
        public string Currency { get; set; }
        public string Amount { get; set; }
    }

    public class Metadata
    {
        public BankSlip BankSlip { get; set; }
        public string ValidationId { get; set; }
    }
    public class BankSlip
    {
        public string ImageUrl { get; set; }
    }
    public class Label
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
    public class FromUser
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }

    }
    public class StatusFacebook
    {
        [JsonProperty("video_status")]
        public string video_status { get; set; }
    }

    public class ChangeValueConverter : JsonConverter<ChangeValue>
    {
        public override ChangeValue ReadJson(JsonReader reader, Type objectType, ChangeValue existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                return serializer.Deserialize<ChangeValue>(reader);
            }
            else if (reader.TokenType == JsonToken.StartArray)
            {
                var payments = serializer.Deserialize<List<Payment>>(reader);
                return new ChangeValue { Payments = payments };
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return new ChangeValue { Message = reader.Value.ToString() };
            }
            else if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString() == "status")
            {
                reader.Read();
                var status = serializer.Deserialize<StatusFacebook>(reader);
                existingValue.Status = status;
                return existingValue;
            }

            return serializer.Deserialize<ChangeValue>(reader);
        }

        public override void WriteJson(JsonWriter writer, ChangeValue value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

}
