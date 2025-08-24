using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.BotFlow.Dtos
{
    public class GetTestBotFlowDto
    {
        public bool IsReStart { get; set; }
        public long IdFlow { get; set; }
        public int TenantId { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public long _ts { get; set; }
        public int ItemType { get; set; }

        public BotTestStepModel BotTestStepModel { get; set; }
        public GetBotFlowForViewDto getBotFlowForViewDto { get; set; }
        
    }
    public class BotTestStepModel
    {

        public List<string> Buttons { get; set; }
        public bool IsButton{ get; set; }
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
        public Dictionary<string, string> UserParmeter { get; set; }=new Dictionary<string, string>();

    }
}
