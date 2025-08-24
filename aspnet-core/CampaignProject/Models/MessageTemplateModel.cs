using Newtonsoft.Json;
using System.Collections.Generic;

namespace CampaignProject.Models
{
    public class WhatsAppMessageTemplateModel
    {
        [JsonProperty("data")]
        public MessageTemplateModel[] data { get; set; }
        [JsonProperty("paging")]
        public PagingModel paging { get; set; }
    }

    public class PagingModel
    {
        [JsonProperty("cursors")]
        public CursorsModel cursors { get; set; }
    }

    public class CursorsModel
    {
        [JsonProperty("before")]
        public string before { get; set; }
        [JsonProperty("after")]
        public string after { get; set; }
    }
    public class MessageTemplateModel
    {
        [JsonProperty("id")]
        public string id { get; set; }


        [JsonProperty("name")]
        public string name { get; set; }


        [JsonProperty("sub_category")]
        public string sub_category { get; set; }

        [JsonProperty("language")]
        public string language { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("category")]
        public string category { get; set; }

        [JsonProperty("components")]
        public List<WhatsAppComponentModel> components { get; set; }

        [JsonProperty("LocalTemplateId")]
        public long LocalTemplateId { get; set; }

        [JsonProperty("mediaType")]
        public string mediaType { get; set; }

        [JsonProperty("mediaLink")]
        public string mediaLink { get; set; }

        [JsonProperty("isDeleted")]
        public bool isDeleted { get; set; }

        [JsonProperty("VariableCount")]
        public int VariableCount { get; set; }

        [JsonProperty("TenantId")]
        public int TenantId { get; set; }

        [JsonProperty("BtnOneActionId")]
        public long? BtnOneActionId { get; set; }
        [JsonProperty("BtnTwoActionId")]
        public long? BtnTwoActionId { get; set; }
        [JsonProperty("BtnThreeActionId")]
        public long? BtnThreeActionId { get; set; }
    }

    public class WhatsAppComponentModel
    {
        

        [JsonProperty("add_security_recommendation")]
        public bool? add_security_recommendation { get; set; }

        [JsonProperty("code_expiration_minutes")]
        public double? code_expiration_minutes { get; set; }


        [JsonProperty("cards")]
        public List<CardsModel> cards { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("format")]
        public string format { get; set; }
        [JsonProperty("example")]
        public WhatsAppExampleModel example { get; set; }

        [JsonProperty("text")]
        public string text { get; set; }

        [JsonProperty("buttons")]
        public List<WhatsAppButtonModel> buttons { get; set; }

        [JsonProperty("parameters")]
        public List<ParametersModel> parameters { get; set; } = null;
    }
    public class CardsModel
    {
        [JsonProperty("components")]
        public List<WhatsAppComponentModel> components { get; set; }

        [JsonProperty("variableCount")]
        public int? variableCount { get; set; }

    }
    public class WhatsAppExampleModel
    {
        [JsonProperty("header_handle")]
        public string[] header_handle { get; set; }
        [JsonProperty("body_text")]
        public string[][] body_text { get; set; }

        [JsonProperty("mediaID")]
        public string mediaID { get; set; }

    }
    public class WhatsAppButtonModel
    {
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("text")]
        public string text { get; set; }
        [JsonProperty("phone_number")]
        public string phone_number { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("example")]
        public List<string> example { get; set; }


    }
    public class WhatsAppTemplateResultModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public bool success { get; set; }
        public Error error { get; set; }
        public class Error
        {
            public string message { get; set; }
            public string type { get; set; }
            public int code { get; set; }
            public int error_subcode { get; set; }
            public bool is_transient { get; set; }
            public string error_user_title { get; set; }
            public string error_user_msg { get; set; }
            public string fbtrace_id { get; set; }
        }
    }


    public class ParametersModel
    {
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("text")]
        public string text { get; set; }
    }


}





