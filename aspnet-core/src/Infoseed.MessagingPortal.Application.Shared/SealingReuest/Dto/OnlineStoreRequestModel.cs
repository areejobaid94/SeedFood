using System;
using System.Collections.Generic;
using System.Text;
using static Infoseed.MessagingPortal.SealingReuest.Dto.RequestEnums;

namespace Infoseed.MessagingPortal.SealingReuest.Dto
{
    public class OnlineStoreRequestModel
    {
        public int NumberOfBranches { get; set; }
        
        public DeliveryPricingMethodEnum deliveryPricingMethodEnum { get; set; }
        public int DeliveryPricingMethodId
        {
            get { return (int)this.deliveryPricingMethodEnum; }
            set { this.deliveryPricingMethodEnum = (DeliveryPricingMethodEnum)value; }
        }
        public string DeliveryPricingMethodName {
            get { return Enum.GetName(typeof(DeliveryPricingMethodEnum), DeliveryPricingMethodId); }
        }

        //public ChatBotLanguageEnum chatBotLanguageEnum { get; set; }
        //public int ChatBotLanguageId
        //{
        //    get { return (int)this.chatBotLanguageEnum; }
        //    set { this.chatBotLanguageEnum = (ChatBotLanguageEnum)value; }
        //}
        //public string ChatBotLanguageName
        //{
        //    get { return Enum.GetName(typeof(ChatBotLanguageEnum), ChatBotLanguageId); }
        //}
        public string chatBotLanguage { get; set; }
        
        public string optionIncluded { get; set; }
        
    }
}
