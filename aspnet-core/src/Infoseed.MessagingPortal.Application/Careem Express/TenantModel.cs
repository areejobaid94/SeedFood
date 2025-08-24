using Framework.Data;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.OrderOffer.Dtos;
using System;


namespace Infoseed.MessagingPortal.Careem_Express
{
    public class TenantModel
    {
        public int? TenantId { get; set; }
        public InfoSeedContainerItemTypes ItemType { get; set; }
        public string SmoochAppID { get; set; }

        public string SmoochSecretKey { get; set; }
        public string SmoochAPIKeyID { get; set; }

        public string DirectLineSecret { get; set; }
        public string botId { get; set; }
        public bool IsBotActive { get; set; }
        public string MassageIfBotNotActive { get; set; }


        //public  DateTime StartDate { get; set; }
        //public  DateTime EndDate { get; set; }
        public bool IsWorkActive { get; set; }
        //public string WorkText { get; set; }
        public WorkModel workModel { get; set; }

        public int? BotTemplateId { get; set; }

        public bool IsCancelOrder { get; set; }
        public int CancelTime { get; set; }

        public bool IsTaxOrder { get; set; }
        public decimal TaxValue { get; set; }

        public bool IsEvaluation { get; set; }

        public string EvaluationText { get; set; }
        public int EvaluationTime { get; set; }


        public string D360Key { get; set; }
        public bool IsD360Dialog { get; set; } = false;

        public string TenancyName { get; set; }
        public string PhoneNumber { get; set; }

        public string PhoneNumberId { get; set; }
        public bool isOrderOffer { get; set; }

        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public int _ts { get; set; }

        public bool IsBundleActive { get; set; }

        public DeliveryCostType DeliveryCostType { get; set; }
        public int DeliveryCostTypeId
        {
            get { return (int)this.DeliveryCostType; }
            set { this.DeliveryCostType = (DeliveryCostType)value; }
        }

        public string AccessToken { get; set; }
        public string WhatsAppAccountID { get; set; }
        public int flag { get; set; }
        public bool IsLiveChatWorkActive { get; set; }
        public WorkModel LiveChatWorkingHours { get; set; } = new WorkModel();

        public OrderOfferDto OneOrderOffern { get; set; } = new OrderOfferDto();
        public CaptionDto OneCaption { get; set; } = new CaptionDto();
        public bool IsLoyalityPoint { get; set; }
        public int Points { get; set; }

        public int BIDailyLimit { get; set; }
        public string CurrencyCode { get; set; }
        public string TimeZone { get; set; }


        public int ReminderBookingHour { get; set; }
        public int BookingCapacity { get; set; }

        public bool IsCaution { get; set; }
        public bool IsPaidInvoice { get; set; }
        public string ZohoCustomerId { get; set; }
        public string UnAvailableBookingDates { get; set; }
        public string ConfirmRequestText { get; set; }
        public string RejectRequestText { get; set; }
        public bool isReplyAfterHumanHandOver { get; set; } = true;

        public string Image { get; set; }

        public string ImageBg { get; set; }


        public string MenuReminderMessage { get; set; }
        public bool IsActiveMenuReminder { get; set; }
        public int TimeReminder { get; set; }

        public bool IsBotLanguageAr { get; set; }
        public bool IsBotLanguageEn { get; set; }
        public bool IsPreOrder { get; set; }
        public bool IsPickup { get; set; }
        public bool IsDelivery { get; set; }
        public bool IsInquiry { get; set; }
        public bool IsMenuLinkFirst { get; set; }
        public bool IsSelectPaymentMethod { get; set; }
        public bool IsBellOn { get; set; }
        public bool IsBellContinues { get; set; }
        public string MerchantID { get; set; }


        public string FacebookPageId { get; set; }
        public string FacebookAccessToken { get; set; }
        public string InstagramId { get; set; }
        public string InstagramAccessToken { get; set; }
        public string CatalogueLink { get; set; }
        public string BusinessId { get; set; }
        public string CatalogueAccessToken { get; set; }
        public string DeliveryType { get; set; }

    }

    public class WorkModelNBot
    {
        public string WorkTextAR { get; set; }
        public string WorkTextEN { get; set; }
        public bool IsWorkActiveSun { get; set; }
        public bool HasSPSun { get; set; }
        public string WorkTextSun { get; set; }
        public string StartDateSun { get; set; }
        public string EndDateSun { get; set; }
        public object StartDateSunSP { get; set; }
        public object EndDateSunSP { get; set; }
        public bool IsWorkActiveMon { get; set; }
        public bool HasSPMon { get; set; }
        public string WorkTextMon { get; set; }
        public object StartDateMon { get; set; }
        public object EndDateMon { get; set; }
        public object StartDateMonSP { get; set; }
        public object EndDateMonSP { get; set; }
        public bool IsWorkActiveTues { get; set; }
        public bool HasSPTues { get; set; }
        public string WorkTextTues { get; set; }
        public object StartDateTues { get; set; }
        public object EndDateTues { get; set; }
        public object StartDateTuesSP { get; set; }
        public object EndDateTuesSP { get; set; }
        public bool IsWorkActiveWed { get; set; }
        public bool HasSPWed { get; set; }
        public string WorkTextWed { get; set; }
        public object StartDateWed { get; set; }
        public object EndDateWed { get; set; }
        public object StartDateWedSP { get; set; }
        public object EndDateWedSP { get; set; }
        public bool IsWorkActiveThurs { get; set; }
        public bool HasSPThurs { get; set; }
        public string WorkTextThurs { get; set; }
        public object StartDateThurs { get; set; }
        public object EndDateThurs { get; set; }
        public object StartDateThursSP { get; set; }
        public object EndDateThursSP { get; set; }
        public bool IsWorkActiveFri { get; set; }
        public bool HasSPFri { get; set; }
        public string WorkTextFri { get; set; }
        public object StartDateFri { get; set; }
        public object EndDateFri { get; set; }
        public object StartDateFriSP { get; set; }
        public object EndDateFriSP { get; set; }
        public bool IsWorkActiveSat { get; set; }
        public bool HasSPSat { get; set; }
        public string WorkTextSat { get; set; }
        public string StartDateSat { get; set; }
        public string EndDateSat { get; set; }
        public string StartDateSatSP { get; set; }
        public string EndDateSatSP { get; set; }
    }

    public class WorkModel
    {



        public bool IsWorkActiveSun { get; set; }
        public bool HasSPSun { get; set; }
        public string WorkTextSun { get; set; }
        public DateTime StartDateSun { get; set; }
        public DateTime EndDateSun { get; set; }
        public DateTime? StartDateSunSP { get; set; }
        public DateTime? EndDateSunSP { get; set; }


        public bool IsWorkActiveMon { get; set; }
        public bool HasSPMon { get; set; }
        public string WorkTextMon { get; set; }
        public DateTime StartDateMon { get; set; }
        public DateTime EndDateMon { get; set; }
        public DateTime? StartDateMonSP { get; set; }
        public DateTime? EndDateMonSP { get; set; }


        public bool IsWorkActiveTues { get; set; }
        public bool HasSPTues { get; set; }
        public string WorkTextTues { get; set; }
        public DateTime StartDateTues { get; set; }
        public DateTime EndDateTues { get; set; }
        public DateTime? StartDateTuesSP { get; set; }
        public DateTime? EndDateTuesSP { get; set; }



        public bool IsWorkActiveWed { get; set; }
        public bool HasSPWed { get; set; }
        public string WorkTextWed { get; set; }
        public DateTime StartDateWed { get; set; }
        public DateTime EndDateWed { get; set; }
        public DateTime? StartDateWedSP { get; set; }
        public DateTime? EndDateWedSP { get; set; }


        public bool IsWorkActiveThurs { get; set; }
        public bool HasSPThurs { get; set; }
        public string WorkTextThurs { get; set; }
        public DateTime StartDateThurs { get; set; }
        public DateTime EndDateThurs { get; set; }
        public DateTime? StartDateThursSP { get; set; }
        public DateTime? EndDateThursSP { get; set; }


        public bool IsWorkActiveFri { get; set; }
        public bool HasSPFri { get; set; }
        public string WorkTextFri { get; set; }
        public DateTime StartDateFri { get; set; }
        public DateTime EndDateFri { get; set; }
        public DateTime? StartDateFriSP { get; set; }
        public DateTime? EndDateFriSP { get; set; }



        public bool IsWorkActiveSat { get; set; }
        public bool HasSPSat { get; set; }
        public string WorkTextSat { get; set; }
        public DateTime StartDateSat { get; set; }
        public DateTime EndDateSat { get; set; }
        public DateTime? StartDateSatSP { get; set; }
        public DateTime? EndDateSatSP { get; set; }

        public string CatalogueLink { get; set; }
        public string BusinessId { get; set; }
        public string CatalogueAccessToken { get; set; }
        public string DeliveryType { get; set; }
    }
}
