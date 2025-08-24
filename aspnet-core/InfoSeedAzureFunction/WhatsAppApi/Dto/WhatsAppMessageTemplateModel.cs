using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.WhatsAppApi.Dto
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



    }

    public class WhatsAppComponentModel
    {
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
    }
    public class WhatsAppExampleModel
    {
        [JsonProperty("header_handle")]
        public string[] header_handle { get; set; }
        [JsonProperty("body_text")]
        public string[][] body_text { get; set; }
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

    }
    public class WhatsAppTemplateResultModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

    }


}








