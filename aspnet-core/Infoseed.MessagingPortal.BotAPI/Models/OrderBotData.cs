using Infoseed.MessagingPortal.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.Constants;

namespace Infoseed.MessagingPortal.BotAPI.Models
{
    public class OrderBotData
    {

        public string AfterBranchCost { get; set; }

        public decimal Total { get; set; }
        public int Id { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string BranchCost { get; set; }
        //public string OrderRemarks { get; set; }
        public string OrderNumber { get; set; }
        //public bool IsDelivery { get; set; }
        public int AreaID { get; set; }
        public string AreaName { get; set; }
        public int TypeChoes { get; set; }
        public string Address { get; set; }

        public string BotLocal { get; set; }

        public int BranchAreaId { get; set; }
        public int LocationID { get; set; }


        public int FromLocationID { get; set; }
        public int ToLocationID { get; set; }


        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }

        public string FromLocationDescribatione { get; set; }
        public string OrderDescribation { get; set; }

        public string SpecialRequest { get; set; }



        public string SelectDay { get; set; }
        public string SelectTime { get; set; }
        public bool IsPreOrder { get; set; }


        public string BranchAreaName { get; set; }
        public string RustrantBranchAreaName { get; set; }





        public string FromAddress { get; set; }
        public string ToAddress { get; set; }


        public string CaptionText { get; set; }
        public string AddressFromation { get; set; }

        public decimal DeliveryCost { get; set; }

        public int ContactId { get; set; }
        public int? TenantID { get; set; }

        public string FromAddressEn { get; set; }
        public string ToAddressEn { get; set; }



        //internal static Order UpdateOrderNumberModel(List<Order> _orders)
        //{
        //    var Number = _orders.Count() + 1;



        //    return new Order()
        //    {
        //        OrderNumber = Number

        //    };
        //}
    }
}
