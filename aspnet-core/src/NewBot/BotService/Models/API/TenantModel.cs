//using System;

//namespace BotService.Models.API
//{
//    public class TenantModel
//    {
//        public int? TenantId { get; set; }
//        public InfoSeedContainerItemTypes ItemType { get; set; }
//        public string SmoochAppID { get; set; }

//        public string SmoochSecretKey { get; set; }
//        public string SmoochAPIKeyID { get; set; }

//        public string DirectLineSecret { get; set; }
//        public string botId { get; set; }
//        public bool IsBotActive { get; set; }

//        //public  DateTime StartDate { get; set; }
//        //public  DateTime EndDate { get; set; }
//        public bool IsWorkActive { get; set; }
//        //public string WorkText { get; set; }
//        public WorkModel workModel { get; set; }

//        public int? BotTemplateId { get; set; }

//        public bool IsCancelOrder { get; set; }
//        public int CancelTime { get; set; }


//        public bool IsEvaluation { get; set; }

//        public string EvaluationText { get; set; }
//        public int EvaluationTime { get; set; }


//        public string D360Key { get; set; }


//        public string PhoneNumber { get; set; }

//        public string PhoneNumberId { get; set; }
//        public bool isOrderOffer { get; set; }

//        public string id { get; set; }
//        public string _rid { get; set; }
//        public string _self { get; set; }
//        public string _etag { get; set; }
//        public string _attachments { get; set; }
//        public int _ts { get; set; }

//        public bool IsBundleActive { get; set; }

//        public DeliveryCostType DeliveryCostType { get; set; }
//        public int DeliveryCostTypeId
//        {
//            get { return (int)this.DeliveryCostType; }
//            set { this.DeliveryCostType = (DeliveryCostType)value; }
//        }

//        public string AccessToken { get; set; }

//        public int flag { get; set; }
//        public bool IsLiveChatWorkActive { get; set; }
//        public WorkModel LiveChatWorkingHours { get; set; } = new WorkModel();

//        public OrderOfferDto OneOrderOffern { get; set; } = new OrderOfferDto();
//        public CaptionDto OneCaption { get; set; } = new CaptionDto();
//        public bool IsLoyalityPoint { get; set; }
//        public int Points { get; set; }

//        public int BIDailyLimit { get; set; }
//        public string CurrencyCode { get; set; }
//        public string TimeZone { get; set; }


//        public bool IsPreOrder { get; set; }
//        public bool IsPickup { get; set; }
//        public bool IsDelivery { get; set; }


//    }

//    public class WorkModelNBot
//    {
//        public string WorkTextAR { get; set; }
//        public string WorkTextEN { get; set; }
//        public bool IsWorkActiveSun { get; set; }
//        public bool HasSPSun { get; set; }
//        public string WorkTextSun { get; set; }
//        public string StartDateSun { get; set; }
//        public string EndDateSun { get; set; }
//        public object StartDateSunSP { get; set; }
//        public object EndDateSunSP { get; set; }
//        public bool IsWorkActiveMon { get; set; }
//        public bool HasSPMon { get; set; }
//        public string WorkTextMon { get; set; }
//        public object StartDateMon { get; set; }
//        public object EndDateMon { get; set; }
//        public object StartDateMonSP { get; set; }
//        public object EndDateMonSP { get; set; }
//        public bool IsWorkActiveTues { get; set; }
//        public bool HasSPTues { get; set; }
//        public string WorkTextTues { get; set; }
//        public object StartDateTues { get; set; }
//        public object EndDateTues { get; set; }
//        public object StartDateTuesSP { get; set; }
//        public object EndDateTuesSP { get; set; }
//        public bool IsWorkActiveWed { get; set; }
//        public bool HasSPWed { get; set; }
//        public string WorkTextWed { get; set; }
//        public object StartDateWed { get; set; }
//        public object EndDateWed { get; set; }
//        public object StartDateWedSP { get; set; }
//        public object EndDateWedSP { get; set; }
//        public bool IsWorkActiveThurs { get; set; }
//        public bool HasSPThurs { get; set; }
//        public string WorkTextThurs { get; set; }
//        public object StartDateThurs { get; set; }
//        public object EndDateThurs { get; set; }
//        public object StartDateThursSP { get; set; }
//        public object EndDateThursSP { get; set; }
//        public bool IsWorkActiveFri { get; set; }
//        public bool HasSPFri { get; set; }
//        public string WorkTextFri { get; set; }
//        public object StartDateFri { get; set; }
//        public object EndDateFri { get; set; }
//        public object StartDateFriSP { get; set; }
//        public object EndDateFriSP { get; set; }
//        public bool IsWorkActiveSat { get; set; }
//        public bool HasSPSat { get; set; }
//        public string WorkTextSat { get; set; }
//        public string StartDateSat { get; set; }
//        public string EndDateSat { get; set; }
//        public string StartDateSatSP { get; set; }
//        public string EndDateSatSP { get; set; }
//    }

//    public class WorkModel
//    {



//        public bool IsWorkActiveSun { get; set; }
//        public bool HasSPSun { get; set; }
//        public string WorkTextSun { get; set; }
//        public DateTime StartDateSun { get; set; }
//        public DateTime EndDateSun { get; set; }
//        public DateTime? StartDateSunSP { get; set; }
//        public DateTime? EndDateSunSP { get; set; }


//        public bool IsWorkActiveMon { get; set; }
//        public bool HasSPMon { get; set; }
//        public string WorkTextMon { get; set; }
//        public DateTime StartDateMon { get; set; }
//        public DateTime EndDateMon { get; set; }
//        public DateTime? StartDateMonSP { get; set; }
//        public DateTime? EndDateMonSP { get; set; }


//        public bool IsWorkActiveTues { get; set; }
//        public bool HasSPTues { get; set; }
//        public string WorkTextTues { get; set; }
//        public DateTime StartDateTues { get; set; }
//        public DateTime EndDateTues { get; set; }
//        public DateTime? StartDateTuesSP { get; set; }
//        public DateTime? EndDateTuesSP { get; set; }



//        public bool IsWorkActiveWed { get; set; }
//        public bool HasSPWed { get; set; }
//        public string WorkTextWed { get; set; }
//        public DateTime StartDateWed { get; set; }
//        public DateTime EndDateWed { get; set; }
//        public DateTime? StartDateWedSP { get; set; }
//        public DateTime? EndDateWedSP { get; set; }


//        public bool IsWorkActiveThurs { get; set; }
//        public bool HasSPThurs { get; set; }
//        public string WorkTextThurs { get; set; }
//        public DateTime StartDateThurs { get; set; }
//        public DateTime EndDateThurs { get; set; }
//        public DateTime? StartDateThursSP { get; set; }
//        public DateTime? EndDateThursSP { get; set; }


//        public bool IsWorkActiveFri { get; set; }
//        public bool HasSPFri { get; set; }
//        public string WorkTextFri { get; set; }
//        public DateTime StartDateFri { get; set; }
//        public DateTime EndDateFri { get; set; }
//        public DateTime? StartDateFriSP { get; set; }
//        public DateTime? EndDateFriSP { get; set; }



//        public bool IsWorkActiveSat { get; set; }
//        public bool HasSPSat { get; set; }
//        public string WorkTextSat { get; set; }
//        public DateTime StartDateSat { get; set; }
//        public DateTime EndDateSat { get; set; }
//        public DateTime? StartDateSatSP { get; set; }
//        public DateTime? EndDateSatSP { get; set; }


    
//    }

//    public enum DeliveryCostType
//    {

//        PerLocation = 0,
//        PerKiloMeter = 1,
//    }

//    public class OrderOfferDto 
//    {
//        public bool isAvailable { get; set; }
//        public bool isPersentageDiscount { get; set; }
//        public DateTime OrderOfferStart { get; set; }
//        public DateTime OrderOfferEnd { get; set; }
//        public string OrderOfferStartS { get; set; }
//        public string OrderOfferEndS { get; set; }

//        public dynamic[,] SelectetDate { get; set; }

//        public DateTime OrderOfferDateStart { get; set; }
//        public DateTime OrderOfferDateEnd { get; set; }
//        public string OrderOfferDateStartS { get; set; }
//        public string OrderOfferDateEndS { get; set; }


//        public string Cities { get; set; }
//        public string Area { get; set; }

//        public decimal FeesStart { get; set; }
//        public decimal FeesEnd { get; set; }

//        public decimal NewFees { get; set; }


//        public bool isBranchDiscount { get; set; }
//        public string BranchesName { get; set; }
//        public string BranchesIds { get; set; }
//    }

//    public class CaptionDto 
//    {
//        public int? TenantId { get; set; }
//        public int Id { get; set; }
//        public string Text { get; set; }
//        public int LanguageBotId { get; set; }
//        public int TextResourceId { get; set; }
//        public string HeaderText { get; set; }

//    }


//    public enum InfoSeedContainerItemTypes
//    {
//        CustomerItem,
//        ConversationItem,
//        Tenant,
//        ConversationBot
//    }
//    public enum CustomerStatus
//    {
//        Active = 1,
//        InActive,
//    }
//    public enum CustomerChatStatus
//    {
//        All = 0,
//        Active = 1,
//        ClosedChats = 2,
//    }
//}
