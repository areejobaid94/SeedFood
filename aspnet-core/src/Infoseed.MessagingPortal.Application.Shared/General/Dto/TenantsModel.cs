//using Framework.Data;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.OrderOffer.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.General.Dto
{
    public class TenantsModel
    {
        public int? TenantId { get; set; }
       // public InfoSeedContainerItemTypes ItemType { get; set; }
        public string SmoochAppID { get; set; }

        public string SmoochSecretKey { get; set; }
        public string SmoochAPIKeyID { get; set; }

        public string DirectLineSecret { get; set; }
        public string botId { get; set; }
        public bool IsBotActive { get; set; }

        //public  DateTime StartDate { get; set; }
        //public  DateTime EndDate { get; set; }
        public bool IsWorkActive { get; set; }
        //public string WorkText { get; set; }
        public WorkModel workModel { get; set; }

        public int? BotTemplateId { get; set; }

        public bool IsCancelOrder { get; set; }
        public int CancelTime { get; set; }


        public bool IsEvaluation { get; set; }

        public string EvaluationText { get; set; }
        public int EvaluationTime { get; set; }

        public bool IsD360Dialog { get; set; }

        public string D360Key { get; set; }
        

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

        public int flag { get; set; }
        public bool IsLiveChatWorkActive { get; set; }
        public WorkModel LiveChatWorkingHours { get; set; } = new WorkModel();

        public OrderOfferDto OneOrderOffern { get; set; } = new OrderOfferDto();
        public CaptionDto OneCaption { get; set; } = new CaptionDto();
        public bool IsLoyalityPoint { get; set; }
        public int Points { get; set; }
        public string ZohoCustomerId { get; set; }

        public bool IsCaution { get; set; } = false;
        public bool IsPaidInvoice { get; set; }

        public string WhatsAppAccountID { get; set; }
        public int CautionDays { get; set; }
        public int WarningDays { get; set; }
        public string UnAvailableBookingDates { get; set; }
        public string ConfirmRequestText { get; set; }
        public string RejectRequestText { get; set; }
        public bool IsReplyAfterHumanHandOver { get; set; } = true;

        public string DomainName { get; set; }
        public string CustomerName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationTime { get; set; }
        public string ActiveStatus => IsActive ? "Active" : "Not Active";


    }
}
