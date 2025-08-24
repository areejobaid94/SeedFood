using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infoseed.MessagingPortal.UTracOrder.Dto
{
    public class UTracCreateOrderModel
    {
        [Required]
        public string integrator_id { get; set; }

        [Required]
        public string integrator_number { get; set; }

        [Required]
        public string payment_method { get; set; } = "CASH";

        [Required]
        public string price_id { get; set; }

        [Required]
        public string order_price { get; set; }

        public string change_amount { get; set; } = "0";

        public string note { get; set; }

        [Required]
        public UTracPersonModel receiver { get; set; }

        [Required]
        public UTracPersonModel sender { get; set; }

    }

    public class UTracPersonModel
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string phone { get; set; }

        public string image { get; set; }

        [Required]
        public float[] location { get; set; }
        [Required]
        public UTracAddressModel address { get; set; }
    }

    public class UTracAddressModel
    {
        public string city { get; set; }

        [Required]
        public string area { get; set; }
        public string street { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string apartment { get; set; }
        public string landmark { get; set; }
        public string postal_code { get; set; }
    }

    public class UTracCreateOrderResponseModel
    {
        public string status { get; set; }
        public UTracCreateOrderResponseDataModel data { get; set; }
    }
    public class UTracCreateOrderResponseDataModel
    {
        public string order_id { get; set; }
        public string order_number { get; set; }
        public string message { get; set; }

    }
}
