using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.BotModel
{
    public class UpdateOrderModel
    {
        public int ContactId { get; set; }
        public int TenantID { get; set; }


        public int OrderId { get; set; }
        public decimal OrderTotal { get; set; }

        public int MenuId { get; set; }
        public bool IsSpecialRequest { get; set; }
        public string SpecialRequest { get; set; }

        public int BranchId { get; set; }
        public string BranchName { get; set; }


        public string TypeChoes { get; set; }     
        public string BotLocal { get; set; }


        public string captionOrderInfoText { get; set; }
        public string captionOrderNumberText { get; set; }
        public string captionTotalOfAllOrderText { get; set; }
        public string captionEndOrderText { get; set; }
        public string aptionAreaNameText { get; set; }

        public string captionBranchCostText { get; set; }
        public string captionFromLocationText { get; set; }



        public bool isOrderOfferCost { get; set; }
        public string Address { get; set; }
        public string AddressEn { get; set; }
        public decimal DeliveryCostAfter { get; set; }
        public decimal DeliveryCostBefor { get; set; }


        public bool IsPreOrder { get; set; }
        public string SelectDay { get; set; }
        public string SelectTime { get; set; }

        public string LocationFrom { get; set; }


        public string BuyType { get; set; }



        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string FloorNo { get; set; }
        public string ApartmentNumber { get; set; }
    }
}
