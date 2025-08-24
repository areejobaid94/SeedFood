using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.OrderDetails.Dtos
{
   public  class CreateSpecificationChoice
    {
        public int Id { get; set; }
        public string SpecificationChoiceDescription { get; set; }
        public string SpecificationChoiceDescriptionEnglish { get; set; }
        public int SpecificationId { get; set; }
        public decimal? Price { get; set; }
        public int UniqueId { get; set; }
        public int SpecificationUniqueId { get; set; }

    }
}
