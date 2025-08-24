using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;



namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class WhatsAppModel
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("entry")]
        public WhatsAppEntryModel[] Entry { get; set; }
    }

    public class WhatsAppEntryModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("changes")]
        public List<WhatsAppChangeModel> Changes { get; set; }
    }

    public class WhatsAppChangeModel
    {
        [JsonProperty("value")]
        public WhatsAppValueModel Value { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }

    }
    public class WhatsAppStatusesModel
    {
        [JsonProperty("conversation")]
        public WhatsAppConversationModel conversation { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("pricing")]
        public WhatsAppPricingModel pricing { get; set; }
        [JsonProperty("recipient_id")]
        public string recipient_id { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("timestamp")]
        public string timestamp { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }


        [JsonProperty("errors")]
        public List<ErrorDetails> Errors { get; set; }

    }
    public class ErrorDetails
    {

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("code")]
        public int code { get; set; }

        [JsonProperty("error_data")]
        public Error_Data error_data { get; set; }

        [JsonProperty("href")]
        public int href { get; set; }
        

    }


    public class Error_Data
    {
        [JsonProperty("details")]
        public string details { get; set; }
    }



    public class WhatsAppConversationModel
    {
        [JsonProperty("expiration_timestamp")]
        public int expiration_timestamp { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("origin")]
        public WhatsAppOriginModel origin { get; set; }
    }

    public class WhatsAppOriginModel
    {
        [JsonProperty("billable")]
        public string type { get; set; }
    }
    public class WhatsAppPricingModel
    {
        [JsonProperty("billable")]
        public bool billable { get; set; }
        [JsonProperty("category")]
        public string category { get; set; }
        [JsonProperty("pricing_model")]
        public string pricing_model { get; set; }
    }

    public class WhatsAppValueModel
    {
        [JsonProperty("statuses")]
        public List<WhatsAppStatusesModel> statuses { get; set; }

        [JsonProperty("messaging_product")]
        public string Messaging_Product { get; set; }

        [JsonProperty("metadata")]
        public WhatsAppMetadataModel Metadata { get; set; }

        [JsonProperty("contacts")]
        public List<WhatsAppContactModel> Contacts { get; set; }

        [JsonProperty("messages")]
        public List<WhatsAppMessageModel> Messages { get; set; }
    }
    public class WhatsAppMetadataModel
    {
        [JsonProperty("display_phone_number")]
        public string display_phone_number { get; set; }

        [JsonProperty("phone_number_id")]
        public string phone_number_id { get; set; }
    }
    public class WhatsAppContactModel
    {
        [JsonProperty("profile")]
        public WhatsAppProfileModel Profile { get; set; }

        [JsonProperty("wa_id")]
        public string Wa_Id { get; set; }
    }

    public class WhatsAppProfileModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class WhatsAppMessageModel
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("interactive")]
        public WhatsAppInteractiveModel interactive { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("text")]
        public WhatsAppTextModel Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("image")]
        public WhatsAppImageModel image { get; set; }

        [JsonProperty("video")]
        public WhatsAppVideoModel video { get; set; }

        [JsonProperty("document")]
        public WhatsAppDocumentModel document { get; set; }

        [JsonProperty("audio")]
        public WhatsAppVoiceModel voice { get; set; }

        [JsonProperty("location")]
        public WhatsAppLocationModel location { get; set; }

        [JsonProperty("mediaUrl")]
        public string mediaUrl { get; set; }



        [JsonProperty("sticker")]
        public Sticker sticker { get; set; }



        [JsonProperty("context")]
        public Context context { get; set; }


        [JsonProperty("referral")]
        public Referral referral { get; set; }

        [JsonProperty("button")]
        public Button button { get; set; }
        [JsonProperty("order")]
        public WhatsAppOrder order { get; set; }

    }

    public class WhatsAppOrder
    {
        [JsonProperty("catalog_id")]
        public string catalog_id { get; set; }
        [JsonProperty("text")]
        public string text { get; set; }
        [JsonProperty("product_items")]
        public List<ProductItems> product_items { get; set; }
    }

    public class ProductItems
    {
        [JsonProperty("product_retailer_id")]
        public string product_retailer_id { get; set; }
        [JsonProperty("quantity")]
        public string quantity { get; set; }
        [JsonProperty("item_price")]
        public string item_price { get; set; }
        [JsonProperty("currency")]
        public string currency { get; set; }
    }
    public class Sticker
    {
        [JsonProperty("id")]
        public string id { get; set; }

    }

    public class Button
    {
        public string payload { get; set; }
        public string text { get; set; }

    }
    public class Referral
    {
        public string source_url { get; set; }
        public string source_id { get; set; }
        public string source_type { get; set; }
        public string headline { get; set; }
        public string body { get; set; }
        public string media_type { get; set; }
        public string image_url { get; set; }
        public string video_url { get; set; }
        public string thumbnail_url { get; set; }
    }
    public class Context
    {
        public string from { get; set; }
        public string id { get; set; }
    }
    public class WhatsAppListReplyModel
    {
        public string description { get; set; }
        public string id { get; set; }
        public string title { get; set; }
    }

    public class WhatsAppInteractiveModel
    {
        [JsonProperty("list_reply")]
        public WhatsAppListReplyModel list_reply { get; set; }
        [JsonProperty("button_reply")]
        public WhatsAppButtonReplyModel button_reply { get; set; }
        public string type { get; set; }
    }
    public class WhatsAppButtonReplyModel
    {
        public string id { get; set; }
        public string title { get; set; }
    }

    public class WhatsAppTextModel
    {
        [JsonProperty("body")]
        public string Body { get; set; }
    }
    public class WhatsAppLocationModel
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string name { get; set; }
        public string address { get; set; }
    }
    public class WhatsAppImageModel
    {
        public string id { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string caption { get; set; }
        public string link { get; set; }
    }
    public class WhatsAppVideoModell
    {
        public string id { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string caption { get; set; }
        public string link { get; set; }
    }

    public class WhatsAppDocumentModell
    {
        public string caption { get; set; }
        public string filename { get; set; }
        public string id { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string link { get; set; }
    }
    public class WhatsAppVoiceModell
    {
        public string id { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string caption { get; set; }
        public string link { get; set; }
    }


    public class WhatsAppContent
    {
        public string type { get; set; }
        public string text { get; set; }
        public string mediaUrl { get; set; }
        public string altText { get; set; }
        public string agentName { get; set; }
        public string agentId { get; set; }
        public string fileName { get; set; }
        public string userId { get; set; }
        public string conversationID { get; set; }
        public int tenantId { get; set; }

        public bool IsButton { get; set; }

        public List<string> Buttons { get; set; }


    }




}