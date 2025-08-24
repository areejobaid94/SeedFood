using System;

namespace Infoseed.MessagingPortal.BotAPI.Models
{
    public class GasOrderingModel
    {

        public class Rootobject
        {
        //    public bool Success { get; set; }
         //   public string Message { get; set; }
            public Datalist[] dataList { get; set; }
        }

        public class Datalist
        {
            public decimal totalPriceWithCarryCost { get; set; } = 0;
            public decimal literPriceWithCarryCost { get; set; } = 0;
            //public decimal companyID { get; set; }
            //public decimal custID { get; set; }
            //public decimal orderNo { get; set; }
            public DateTime orderDate { get; set; }
            public DateTime order_DeliveryDateTime { get; set; }
            //public decimal itemID { get; set; }
            //public decimal driverNo { get; set; }
            //public decimal carID { get; set; }
          //  public decimal? stationNo { get; set; }
          //  public decimal order_Qty { get; set; }
            public decimal delivered_Qty { get; set; }
            public float itemPrice { get; set; }
            //public decimal carryCostByFiles { get; set; }
            //public decimal carryCost { get; set; }
            //public float order_Amount { get; set; }
            //public float delivered_Amount { get; set; }
            //public string notes { get; set; }
            //public decimal orderStatus { get; set; }
            //public decimal paymentsTypeID { get; set; }
            //public DateTime? voidDate { get; set; }
            //public string voidReason { get; set; }
            //public DateTime? closeDate { get; set; }
            //public string cardNo { get; set; }
            //public string gpsx { get; set; }
            //public string gpsy { get; set; }
            //public string custName { get; set; }
            //public string countriesArNAme { get; set; }
            //public string areasArNAme { get; set; }
            //public string countriesEnName { get; set; }
            //public object areasEnName { get; set; }
            //public object street { get; set; }
            //public string paymentsTypesArName { get; set; }
            //public string paymentsTypesEnName { get; set; }
            //public string driverName { get; set; }
            //public decimal flag { get; set; }
            //public decimal expr1 { get; set; }
            //  public DateTime order_DeliveryDateTime_To { get; set; }
            //  public bool isBlackListOrder { get; set; }
            //  public decimal comm_ID { get; set; }
            //   public string arName { get; set; }
            // public string orderStatusValue { get; set; }
            public string itemName { get; set; }
        }

    }
}
