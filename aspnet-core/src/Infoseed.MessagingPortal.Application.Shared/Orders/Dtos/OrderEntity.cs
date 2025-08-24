using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class OrderEntity
    {
        public List<GetOrderForViewDto> lstOrder { get; set; }
        public int TotalCount { get; set; }
    }
}
