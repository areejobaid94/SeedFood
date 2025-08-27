

namespace Infoseed.MessagingPortal.Careem_Express
{
    public class CreateDeliveryDTO
    {
        public long orderNumber { get; set; }
        public int tenantId { get; set; }
        public double customerLongitude { get; set; }
        public double customerLatitude { get; set; }
        public string pickupNote { get; set; }
        public string customerPhoneNumber { get; set; }
        public string customerBuildingNo { get; set; } = null;
        public string customerBuildingName { get; set; } = null;
        public string customerStreet { get; set; } = null;
        public string merchantOrderNumber { get; set; } = null;
    }

}
