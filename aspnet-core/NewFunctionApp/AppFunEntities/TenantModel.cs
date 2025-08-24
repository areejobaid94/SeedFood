using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewFunctionApp
{
    public class TenantModel
    {
        public int? Id { get; set; }
        public int? TenantId { get; set; }
        public ContainerItemTypes ItemType { get; set; }
        public string SmoochAppID { get; set; }
        public string SmoochSecretKey { get; set; }
        public string SmoochAPIKeyID { get; set; }

        public string DirectLineSecret { get; set; }
        public string botId { get; set; }
        public bool IsBotActive { get; set; }
        public string MassageIfBotNotActive { get; set; }

        public bool IsCancelOrder { get; set; }
        public int CancelTime { get; set; }



        public bool IsWorkActive { get; set; }

        public WorkModel workModel { get; set; }

        public WorkModel LiveChatWorkingHours { get; set; }

        public string D360Key { get; set; }
        public bool IsD360Dialog { get; set; }

        public bool IsEvaluation { get; set; }

        public string EvaluationText { get; set; }
        public int EvaluationTime { get; set; }


        public bool isOrderOffer { get; set; }

        public bool IsLoyalityPoint { get; set; }
        public int Points { get; set; }

        public string PhoneNumber { get; set; }

        public string Image { get; set; }

        public string ImageBg { get; set; }
        public bool IsBundleActive { get; set; }
        public bool IsLiveChatWorkActive { get; set; }

        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public int _ts { get; set; }
        public int DeliveryCostTypeId { get; set; }
        public string AccessToken { get; set; }
        public string WhatsAppAccountID { get; set; }

        public string WhatsAppAppID { get; set; }

        public int? BotTemplateId { get; set; }
        public int BIDailyLimit { get; set; }
        public int DailyLimit { get; set; }

        public bool IsBellOn { get; set; }
        public bool IsBellContinues { get; set; }

        public string CurrencyCode { get; set; }
        public string TimeZoneId { get; set; }
        public bool IsPreOrder { get; set; }
        public bool IsPickup { get; set; }
        public bool IsDelivery { get; set; }
        public string ZohoCustomerId { get; set; }
      
        
        public int ReminderBookingHour { get; set; }
        public int BookingCapacity { get; set; }

        public bool IsCaution { get; set; }
        public bool IsPaidInvoice { get; set; }



        public int CautionDays { get; set; }
        public int WarningDays { get; set; }

        public string UnAvailableBookingDates { get; set; }
        public string ConfirmRequestText { get; set; }
        public string RejectRequestText { get; set; }



        public string MenuReminderMessage { get; set; }
        public bool IsActiveMenuReminder { get; set; }
        public int TimeReminder { get; set; }

        public bool IsBotLanguageAr { get; set; }
        public bool IsBotLanguageEn { get; set; }
        public bool IsInquiry { get; set; }
        public bool IsMenuLinkFirst { get; set; }
        public bool IsSelectPaymentMethod { get; set; }

    }
}
