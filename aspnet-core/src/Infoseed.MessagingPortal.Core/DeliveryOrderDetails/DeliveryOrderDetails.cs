
using System.ComponentModel.DataAnnotations.Schema;


namespace Infoseed.MessagingPortal.DeliveryOrderDetails
{
    [Table("DeliveryOrderDetails")]
   public  class DeliveryOrderDetails
    {
        public int? TenantId { get; set; }

        public int Id { get; set; }

        public int FromLocationId { get; set; }
        public string FromAddress { get; set; }
        public string FromGoogleURL { get; set; }

        public int ToLocationId { get; set; }
        public string ToAddress { get; set; }
        public string ToGoogleURL { get; set; }

        public decimal DeliveryCost { get; set; }
        public string DeliveryCostString { get; set; }

        public  int OrderId { get; set; }

    }
}
