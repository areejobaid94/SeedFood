using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace WebHookStatusFun.Models
{
    public class ListContactToCampin
    {
        public int Id { get; set; }
        public int CustomerOPT { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public Dictionary<string, string> variables { get; set; }
        public TemplateVariablles templateVariables { get; set; }
        [JsonProperty("HeaderVariabllesTemplate")]
        public HeaderVariablesTemplate haderVariablesTemplate { get; set; }
        public FirstButtonURLVariabllesTemplate firstButtonURLVariabllesTemplate { get; set; }
        public SecondButtonURLVariabllesTemplate secondButtonURLVariabllesTemplate { get; set; }
        [JsonPropertyName("CarouselTemplate")]
        public CarouselVariabllesTemplate carouselVariabllesTemplate { get; set; }
        [JsonPropertyName("ButtonCopyCodeVariabllesTemplate")]
        public ButtonCopyCodeVariabllesTemplate buttonCopyCodeVariabllesTemplate { get; set; }
    }


    public class TemplateVariablles
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
        public string VaSixteen { get; set; }
        public string VarSixteen { get; set; }


    }
    public class HeaderVariablesTemplate
    {
        public string VarOne { get; set; }

    }
    public class FirstButtonURLVariabllesTemplate
    {
        public string VarOne { get; set; }
    }
    public class SecondButtonURLVariabllesTemplate
    {
        public string VarOne { get; set; }
    }

    public class ButtonCopyCodeVariabllesTemplate
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
        public SecondButtonURLVariabllesTemplate firstButtonURLVariabllesTemplate { get; set; }
        [JsonPropertyName("firstButtonURLVariabllesTemplate")]
        public FirstButtonURLVariabllesTemplate secondButtonURLVariabllesTemplate { get; set; }
        [JsonPropertyName("CardIndex")]
        public int CardIndex { get; set; }
        [JsonPropertyName("VariableCount")]
        public int VariableCount { get; set; }
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
