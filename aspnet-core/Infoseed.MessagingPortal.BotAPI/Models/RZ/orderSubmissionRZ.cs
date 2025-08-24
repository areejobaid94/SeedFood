using System.Collections.Generic;

namespace Infoseed.MessagingPortal.BotAPI.Models.RZ
{
    public class orderSubmissionRZ
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string lang { get; set; }
        public List<DataModel> data { get; set; }
    }
    public class DataModel
    {
        public string accNo { get; set; }
        public string invNote { get; set; }
        public List<OrderDetailRZ> details{ get; set; }
    }
    public class OrderDetailPayload
    {
        public string itemCode { get; set; }
        public int unitCode { get; set; }
        public double qty { get; set; }
        public double price { get; set; }
    }
    public class OrderDetailRZ
    {
        public string itemCode { get; set; }
        public int unitCode { get; set; }  
        public double qty { get; set; }
        public double price { get; set; }
        public string itemDesc { get; set; }


    }
    public class OrderResponseRZ
    {
        public string message { get; set; }
        public int status { get; set; }
    }
}
