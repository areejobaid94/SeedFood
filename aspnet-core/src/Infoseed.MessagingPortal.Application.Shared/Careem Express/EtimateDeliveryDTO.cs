
namespace Infoseed.MessagingPortal.Careem_Express
{
    public class EtimateDeliveryDTO
    {
        public string type { get; set; }
        public Pickup pickup { get; set; }
        public Dropoff dropoff { get; set; }

        public class Pickup
        {
            public Coordinate coordinate { get; set; }
        }

        public class Coordinate
        {
            public string latitude { get; set; }
            public string longitude { get; set; }
        }

        public class Dropoff
        {
            public Coordinate coordinate { get; set; }
        }

    }
}

