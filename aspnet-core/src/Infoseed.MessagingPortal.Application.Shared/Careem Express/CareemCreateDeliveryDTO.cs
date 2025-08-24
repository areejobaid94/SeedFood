
namespace Infoseed.MessagingPortal.Careem_Express
{
    public class CareemCreateDeliveryDTO
    {
        public double? volume { get; set; }
        public string delivery_action { get; set; }
        public string type { get; set; }
        public Pickup pickup { get; set; }
        public Dropoff dropoff { get; set; }
        public string driver_notes { get; set; }
        public Customer customer { get; set; }
        public Outlet outlet { get; set; }
        public Cash_Collection cash_collection { get; set; }
        public Order order { get; set; }
        //public Delivery_Window delivery_window { get; set; }
    }

    public class Pickup
    {
        public Coordinate coordinate { get; set; }
        public string area { get; set; }
        public string building_name { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string notes { get; set; }
    }

    public class Coordinate
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class Dropoff
    {
        public Coordinate coordinate { get; set; }
        public string area { get; set; }
        public string building_name { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string notes { get; set; }
    }

    public class Customer
    {
        public string name { get; set; }
        public string phone_number { get; set; }
    }

    public class Outlet
    {
        public string name { get; set; }
        public string phone_number { get; set; }
    }

    public class Cash_Collection
    {
        public double amount { get; set; } // order + delivery + 0.20
        public string payment_type { get; set; }
    }

    public class Order
    {
        public string reference { get; set; }
    }

    public class Delivery_Window  //for scheduled delivery
    {
        public string date { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }
}
