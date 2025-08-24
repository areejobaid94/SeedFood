using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.OrderDetails.Dtos
{
   public  class CreateItemSpecifications
    {

        public CreateItemSpecifications()
        {
            SpecificationChoices = new List<CreateSpecificationChoice>();
        }
        public int Id { get; set; }

        public int ItemId { get; set; }
        public int UniqueId { get; set; }

        public string SpecificationDescription { get; set; }
        public string SpecificationDescriptionEnglish { get; set; }
        public List<CreateSpecificationChoice> SpecificationChoices { get; set; }
    }
}
