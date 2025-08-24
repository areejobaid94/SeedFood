using Infoseed.MessagingPortal.WhatsApp.Dto;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace CampaignProject.Models
{
    public class ListContactCampaign
    {
        public int Id { get; set; }
        public int CustomerOPT { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }

        public string PostBody { get; set; }
        public TemplateVariablles2 templateVariables { get; set; }


        [JsonPropertyName("carouselVariabllesTemplate")]
        public CarouselVariabllesTemplate carouselVariabllesTemplate { get; set; }

        [JsonPropertyName("HeaderVariabllesTemplate")]
        public HeaderVariablesTemplate2 headerVariabllesTemplate { get; set; }

        [JsonPropertyName("firstButtonURLVariabllesTemplate")]
        public FirstButtonURLVariabllesTemplate2 FirstButtonURLVariabllesTemplate { get; set; }

        [JsonPropertyName("secondButtonURLVariabllesTemplate")]
        public SecondButtonURLVariabllesTemplate2 SecondButtonURLVariabllesTemplate { get; set; }

        [JsonPropertyName("buttonCopyCodeVariabllesTemplate")]
        public ButtonCopyCodeVariabllesTemplate2 buttonCopyCodeVariabllesTemplate { get; set; }



    }
    public class TemplateVariablles2
    {
        public string VarOne { get; set; }
        public string VarTwo { get; set; }
        public string VarThree { get; set; }
        public string VarFour { get; set; }
        public string VarFive { get; set; }
        public string VarSix { get; set; }
        public string VarSeven { get; set; }
        public string VarEight { get; set; }
        public string VarNine { get; set; }
        public string VarTen { get; set; }
        public string VarEleven { get; set; }
        public string VarTwelve { get; set; }
        public string VarThirteen { get; set; }
        public string VarFourteen { get; set; }
        public string VarFifteen { get; set; }
    }

    public class HeaderVariablesTemplate2
    {
        public string VarOne { get; set; }
    }
    public class FirstButtonURLVariabllesTemplate2
    {
        public string VarOne { get; set; }
    }
    public class SecondButtonURLVariabllesTemplate2
    {
        public string VarOne { get; set; }
    }


    public class ButtonCopyCodeVariabllesTemplate2
    {
        public string VarOne { get; set; }
    }



      public class CarouselVariabllesTemplate
    {
        public List<Card> cards { get; set; }
     
    }
    public class Card
    {
        [JsonPropertyName("Variables")]
        public CardVariabllesTemplate Variables { get; set; }
        [JsonPropertyName("secondButtonURLVariabllesTemplate")]
        public SecondButtonURLVariabllesTemplate secondButtonURLVariabllesTemplate { get; set; }
        [JsonPropertyName("firstButtonURLVariabllesTemplate")]
        public FirstButtonURLVariabllesTemplate firstButtonURLVariabllesTemplate { get; set; }
        [JsonPropertyName("CardIndex")]
        public int CardIndex { get; set; }
        [JsonPropertyName("VariableCount")]
        public int VariableCount { get; set; }
                [JsonPropertyName("ButtonCopyCodeVariabllesTemplate")]
        public ButtonCopyCodeVariabllesTemplate buttonCopyCodeVariabllesTemplate { get; set; }


    }
    public class CardVariabllesTemplate
    {
        public string VarOne { get; set; }
        public string VarTwo { get; set; }
        public string VarThree { get; set; }
        public string VarFour { get; set; }
        public string VarFive { get; set; }
        public string VarSix { get; set; }
        public string VarSeven { get; set; }
        public string VarEight { get; set; }
        public string VarNine { get; set; }
        public string VarTen { get; set; }
        public string VarEleven { get; set; }
        public string VarTwelve { get; set; }
        public string VarThirteen { get; set; }
        public string VarFourteen { get; set; }
        public string VarFifteen { get; set; }
    }

    public class CarouselCard
    {
        public List<WhatsAppComponentModel> Components { get; set; } = new List<WhatsAppComponentModel>();
        public int VariableCount { get; set; }

        // Helper property to get the BODY component
        public WhatsAppComponentModel BodyComponent =>
            Components?.FirstOrDefault(c => c.type == "BODY");
    }




}
