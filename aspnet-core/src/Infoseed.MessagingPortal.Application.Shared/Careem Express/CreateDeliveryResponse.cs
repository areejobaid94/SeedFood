
namespace Infoseed.MessagingPortal.Careem_Express
{
    public class CreateDeliveryResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public string status_details { get; set; }
        public int reference { get; set; }
        public string carrier_reference { get; set; }
        public PricingCareem pricing { get; set; }
        public int delivery_distance_in_Km { get; set; }
        public int no_of_vehicles_dispatched { get; set; }
        public string proof_of_delivery_image_url { get; set; }
        public string proof_type { get; set; }
        public Related_Deliveries[] related_deliveries { get; set; }
        public string expected_pickup_time { get; set; }
        public string external_order_reference { get; set; }
    }

    public class PricingCareem
    {
        public float trip_cost { get; set; }
        public string currency { get; set; }
    }

    public class Related_Deliveries
    {
        public string id { get; set; }
        public string status { get; set; }
        public int reference { get; set; }
        public PricingCareem pricing { get; set; }
        public int delivery_distance_in_Km { get; set; }
        public string proof_of_delivery_image_url { get; set; }
        public string proof_type { get; set; }
    }

}
