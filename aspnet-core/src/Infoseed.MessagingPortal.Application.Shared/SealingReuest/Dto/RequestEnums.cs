using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.SealingReuest.Dto
{
    public class RequestEnums
    {
        public enum ServiceTypeEnum 
        {
            WhatsAppOnlineStore,
            WhatsAppCampaign,
            WhatsAppBooking,
            EnterpriseCustomBot
        }

        public enum DeliveryPricingMethodEnum 
        {
            PerKilometer,
            PerZones
        }
        public enum PricingEndPlanEnum
        {
           Monthly,
           Annual
        }
        public enum ChatBotLanguageEnum
        {
            Arabic,
            English
        }
        public enum OptionIncludedEnum
        {
            Delivery,
            Takeaway,
            Pre_Order,
            Inquiry
        }
    }
}
