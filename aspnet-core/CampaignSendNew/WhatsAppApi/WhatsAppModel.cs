using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;



namespace CampaignSendNew.WhatsAppApi
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
    public class WhatsAppVideoModel
    {
        public string id { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string caption { get; set; }
        public string link { get; set; }
    }



    public class WhatsAppDocumentModel
    {
        public string caption { get; set; }
        public string filename { get; set; }
        public string id { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string link { get; set; }
    }
    public class WhatsAppVoiceModel
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
       

    }
}