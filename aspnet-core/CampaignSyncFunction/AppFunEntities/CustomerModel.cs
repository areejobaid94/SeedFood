using CampaignSyncFunction.AppFunEntities;
using System;
using System.Collections.Generic;

namespace CampaignSyncFunction
{
    public class CustomerModel
    {
        public string TennantPhoneNumberId { get; set; }
        public string interactiveId { get; set; }
        public int ConversationsCount { get; set; }
        public int DeliveryOrder { get; set; }
        public int TakeAwayOrder { get; set; }
        public int TotalOrder { get; set; }
        public int loyalityPoint { get; set; }

        public string ContactID { get; set; }
        public bool IsSelectedPage { get; set; } = false;
        public bool IsComplaint { get; set; }
        public bool IsBotCloseChat { get; set; }
        public bool IsBotChat { get; set; }
        public bool IsSupport { get; set; }

        public bool IsNewContact { get; set; }
        public bool IsBlock { get; set; }
        public CustomerChat customerChat { get; set; }
        public string blockByAgentName { get; set; }
        public bool isBlockCustomer { get; set; }
        public DateTime? lastNotificationsData { get; set; }
        public string notificationsText { get; set; }
        public string notificationID { get; set; }
        public int UnreadMessagesCount { get; set; }
        public int agentId { get; set; }
        public string userId { get; set; }
        public string avatarUrl { get; set; }
        public string displayName { get; set; }
        public string nickName { get; set; }
        public string phoneNumber { get; set; }
        public string type { get; set; }
        public string MicrosoftBotId { get; set; }
        public string ConversationId { get; set; }

        public string D360Key { get; set; }
        public string SunshineAppID { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime? LastMessageData { get; set; }
        public DateTime? LastConversationStartDateTime { get; set; }
        public string LastMessageText { get; set; }
        public bool IsLockedByAgent { get; set; }
        public bool IsConversationExpired { get; set; }
        public string LockedByAgentName { get; set; }



        public bool IsOpen { get; set; }

        public string Website { get; set; }

        public string EmailAddress { get; set; }

        public string Description { get; set; }
        public bool IsNew { get; set; }

        public int? TenantId { get; set; }


        public int LiveChatStatus { get; set; }
        public string LiveChatStatusName { get; set; }
        public bool IsliveChat { get; set; }
        public DateTime? requestedLiveChatTime { get; set; }


        public DateTime? OpenTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public string Department { get; set; }


        public DateTime? OpenTimeTicket { get; set; }
        public DateTime? CloseTimeTicket { get; set; }
        public string TicketSummary { get; set; }

        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public long _ts { get; set; }
        public CustomerStepModel CustomerStepModel { get; set; }

        public int creation_timestamp { get; set; }
        public int expiration_timestamp { get; set; }


        public ContainerItemTypes ItemType { get; } = ContainerItemTypes.CustomerItem;
        public CustomerStatus CustomerStatus { get; set; }
        public int CustomerStatusID
        {
            get { return (int)this.CustomerStatus; }
            set { this.CustomerStatus = (CustomerStatus)value; }
        }
        public CustomerChatStatus CustomerChatStatus { get; set; }
        public int CustomerChatStatusID
        {
            get { return (int)this.CustomerChatStatus; }
            set { this.CustomerChatStatus = (CustomerChatStatus)value; }
        }






        public long AssignedToUserId { get; set; }
        public string AssignedToUserName { get; set; }
        public string DepartmentUserIds { get; set; }
        public long IdLiveChat { get; set; }





        public int DurationTime { get; set; }

        public int CustomerOPT { get; set; }



        public BotBookingParmeterModel BotBookingParmeterModel { get; set; }
        public List<Microsoft.Bot.Connector.DirectLine.Attachment> attachments { get; set; }





        public GetBotFlowForViewDto getBotFlowForViewDto { get; set; }


        public string templateId { get; set; }
        public string campaignId { get; set; }

        public DateTime? TemplateFlowDate { get; set; }

        public bool IsTemplateFlow { get; set; }
        public bool IsEvaluation { get; set; }

        public Dictionary<string, string> OneTimeQuestionIds { get; set; } = new Dictionary<string, string>();

    }


    public class CustomerStepModel
    {
        public int LangId { get; set; }
        public string LangString { get; set; } = "ar";
        public int ChatStepId { get; set; }
        public int ChatStepPervoiusId { get; set; }
        public int ChatStepNextId { get; set; }
        public int ChatStepLevelId { get; set; }
        public int ChatStepLevelPreviousId { get; set; }
        public long SelectedAreaId { get; set; }
        public long OrderId { get; set; }
        public long OrderNumber { get; set; }
        public string OrderTypeId { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal OrderDeliveryCost { get; set; }
        public int PageNumber { get; set; }

        public string Address { get; set; }

        public decimal? DeliveryCostAfter { get; set; }
        public decimal? DeliveryCostBefor { get; set; }

        public bool isOrderOfferCost { get; set; }

        public long LocationId { get; set; }
        public string LocationAreaName { get; set; }
        public string AddressLatLong { get; set; }
        public bool IsLinkMneuStep { get; set; }
        public bool IsNotSupportLocation { get; set; }
        public decimal Discount { get; set; }

        public int BotCancelOrderId { get; set; }
        public DateTime? OrderCreationTime { get; set; }
        public string AgentIds { get; set; }
        public decimal TotalPoints { get; set; }
        public decimal TotalCreditPoints { get; set; }


        public string SelectDay { get; set; }
        public string SelectTime { get; set; }
        public bool IsPreOrder { get; set; }
        public string BayType { get; set; }


        public bool IsLiveChat { get; set; }

        public string EvaluationQuestionText { get; set; }
        public string EvaluationsReat { get; set; }

        public bool IsItemOffer { get; set; }
        public decimal? ItemOffer { get; set; }

        public bool IsDeliveryOffer { get; set; }
        public decimal? DeliveryOffer { get; set; }

        public string Department1 { get; set; }
        public string Department2 { get; set; }


        public int AssetStepId { get; set; }
        public int AssetlevelId { get; set; }



        public int AssetLevelOneId { get; set; }
        public int AssetLevelTowId { get; set; }
        public int AssetLevelThreeId { get; set; }
        public bool IsAssetOffer { get; set; }


        public string SelectName { get; set; }
        public bool IsPic { get; set; }

        public string selectTypeNumber { get; set; }

        public AttachmentBotModel[] attachmentModels { get; set; }

    }

    public class AttachmentBotModel
    {
        public string contentUrl { get; set; }
        public string contentType { get; set; }
        public string contentName { get; set; }
        // public int Length { get; set; }

    }

    public class BotBookingParmeterModel
    {

        public string UserIds { get; set; }
        public long AreaId { get; set; }

        public string selectDoctor { get; set; }

        public long selectDoctorId { get; set; }

        public string BookingDate { get; set; }
        public string BookingTime { get; set; }
        public string Note { get; set; }

        public string ContactBookingTime { get; set; }
        public long ContactBookingId { get; set; }


    }

    public class BotParmeterModel
    {

        public int TennantId { get; set; }
        public string ContactId { get; set; }
        public int LangId { get; set; }
        public string PhoneNumber { get; set; }
        public int AreaId { get; set; }


    }



}
