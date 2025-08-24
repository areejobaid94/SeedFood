using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class CreateOrderDetailsModel
    {

        public CreateOrderDetailsModel()
        {
            ItemSpecifications = new List<CreateItemSpecifications>();
            createExtraOrderDetailsModels = new List<CreateExtraOrderDetailsModel>();
        }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }

        public int ItemId { get; set; }

        public bool IsCondiments { get; set; }
        public bool IsDeserts { get; set; }
        public bool IsCrispy { get; set; }
        public List<CreateExtraOrderDetailsModel> createExtraOrderDetailsModels { get; set; }

        public List<CreateItemSpecifications> ItemSpecifications { get; set; }

       

    }
}
