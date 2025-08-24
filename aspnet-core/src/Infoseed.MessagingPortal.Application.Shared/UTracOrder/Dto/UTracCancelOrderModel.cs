using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.UTracOrder.Dto
{

    public class UTracCancelOrderModel
    {
        public string integrator_id { get; set; }
        public string integrator_number { get; set; }
        public string cancel_note { get; set; }
    }
    public class UTracCancelOrderResponseModel
    {
        public string status { get; set; }
        public UTracCancelOrderResponseDataModel data { get; set; }
    }
    public class UTracCancelOrderResponseDataModel
    {
        public string message { get; set; }
    }
    
}
