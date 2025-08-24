using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class CreateOrderModel
    {
        public int? TenantId { get; set; }
        public int? CustomerId { get; set; }

        public List<CreateOrderDetailsModel> CreateOrderDetailsModels { get; set; }

        public decimal Total { get; set; }

        public bool? IsZeedlyOrder { get; set; }

    }
}
