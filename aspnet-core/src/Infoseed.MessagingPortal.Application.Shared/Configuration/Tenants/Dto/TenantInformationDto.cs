using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Loyalty.Dto;
using Infoseed.MessagingPortal.OrderOffer.Dtos;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Configuration.Tenants.Dto
{
    public class TenantInformationDto
    {
        public bool IsBellOn { get; set; }
        public bool IsBellContinues { get; set; }

        public int TenantId { get; set; }

        public bool IsWorkActive { get; set; }
        public WorkModel workModel { get; set; } = new WorkModel();

        public bool IsBotActive { get; set; }
        public string MassageIfBotNotActive { get; set; }

        public bool IsCancelOrder { get; set; }

        public int CancelTime { get; set; }

        public bool IsTaxOrder { get; set; }
        public decimal TaxValue { get; set; }

        public bool IsEvaluation { get; set; }
        public string EvaluationText { get; set; }
        public int EvaluationTime { get; set; }

        public bool isOrderOffer { get; set; }

        public List<CaptionDto>  Captions { get; set; } = new List<CaptionDto>();
     
        public List<OrderOfferDto>  OrderOffers { get; set; } = new List<OrderOfferDto>();

        public int flag { get; set; }
        public bool IsLiveChatWorkActive { get; set; }
        public WorkModel LiveChatWorkingHours { get; set; } = new WorkModel();

        public OrderOfferDto OneOrderOffern { get; set; } = new OrderOfferDto();
        public CaptionDto OneCaption { get; set; } = new CaptionDto();

        public bool IsLoyalityPoint { get; set; }
        public int Points { get; set; }

        public bool IsBundleActive { get; set; }

        public LoyaltyModel loyaltyModel { get; set; }

        public DeliveryCostType DeliveryCostType { get; set; }
        public int DeliveryCostTypeId
        {
            get { return (int)this.DeliveryCostType; }
            set { this.DeliveryCostType = (DeliveryCostType)value; }
        }

        public int ReminderBookingHour { get; set; }
        public int BookingCapacity { get; set; }
        public string UnAvailableBookingDates { get; set; }
        public string ConfirmRequestText { get; set; }
        public string RejectRequestText { get; set; }
        public bool IsReplyAfterHumanHandOver { get; set; } = true;
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


        public string ClientIpAddress { get; set; }

    }


    public class WorkModel
    {

        public string WorkTextAR { get; set; }
        public string WorkTextEN { get; set; }


        public bool IsWorkActiveSun { get; set; }
        public bool HasSPSun { get; set; }
        public string WorkTextSun { get; set; }
        public dynamic StartDateSun { get; set; }
        public dynamic EndDateSun { get; set; }
        public dynamic StartDateSunSP { get; set; }
        public dynamic EndDateSunSP { get; set; }



        public bool IsWorkActiveMon { get; set; }
        public bool HasSPMon { get; set; }
        public string WorkTextMon { get; set; }
        public dynamic StartDateMon { get; set; }
        public dynamic EndDateMon { get; set; } 
        public dynamic StartDateMonSP { get; set; }
        public dynamic EndDateMonSP { get; set; }


        public bool IsWorkActiveTues { get; set; }
        public bool HasSPTues { get; set; }
        public string WorkTextTues { get; set; }
        public dynamic StartDateTues { get; set; }
        public dynamic EndDateTues { get; set; }
        public dynamic StartDateTuesSP { get; set; }
        public dynamic EndDateTuesSP { get; set; }



        public bool IsWorkActiveWed { get; set; }
        public bool HasSPWed { get; set; }
        public string WorkTextWed { get; set; }
        public dynamic StartDateWed { get; set; }
        public dynamic EndDateWed { get; set; }
        public dynamic StartDateWedSP { get; set; }
        public dynamic EndDateWedSP { get; set; }



        public bool IsWorkActiveThurs { get; set; }
        public bool HasSPThurs { get; set; }
        public string WorkTextThurs { get; set; }
        public dynamic StartDateThurs { get; set; }
        public dynamic EndDateThurs { get; set; }
        public dynamic StartDateThursSP { get; set; }
        public dynamic EndDateThursSP { get; set; }


        public bool IsWorkActiveFri { get; set; }
        public bool HasSPFri { get; set; }
        public string WorkTextFri { get; set; }
        public dynamic StartDateFri { get; set; }
        public dynamic EndDateFri { get; set; }
        public dynamic StartDateFriSP { get; set; }
        public dynamic EndDateFriSP { get; set; }

  
        public bool IsWorkActiveSat { get; set; }
        public bool HasSPSat { get; set; }
        public string WorkTextSat { get; set; }
        public dynamic StartDateSat { get; set; }
        public dynamic EndDateSat { get; set; } 
        public dynamic StartDateSatSP { get; set; }
        public dynamic EndDateSatSP { get; set; }


    }


 
    }
