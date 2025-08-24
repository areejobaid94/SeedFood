using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.OrderDetails.Dtos
{
    public class CreateExtraOrderDetailsModel
    {

        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public int ItemId { get; set; }

        public int id { get; set; }

        public int itemAdditionsCategoryId { get; set; }

    }
}
