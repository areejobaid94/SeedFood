using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Careem_Express
{
    public class GetDeliveryResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public string status_details { get; set; }
        public int reference { get; set; }
        public string carrier_reference { get; set; }
        public PricingCareem pricing { get; set; } = null;
        public int delivery_distance_in_Km { get; set; }
        public string proof_of_delivery_image_url { get; set; }
        public string proof_type { get; set; }
        public string external_order_reference { get; set; }
    }


}
