using System.Collections.Generic;

namespace BotService.Models.API
{
    public class ItemSpecification
    {
        public ItemSpecification()
        {
            SpecificationChoices = new List<SpecificationChoice>();
        }
        public int Id { get; set; }
        public int ItemSpecificationId { get; set; }
        public string SpecificationDescription { get; set; }
        public string SpecificationDescriptionEnglish { get; set; }
        public bool IsMultipleSelection { get; set; }
        public bool IsRequired { get; set; }

        public int MaxSelectNumber { get; set; }
        //public bool IsInService { get; set; }

        public int TenantId { get; set; }
        public int Priority { get; set; }

        public int UniqueId { get; set; }
        public List<SpecificationChoice> SpecificationChoices { get; set; }
    }
}
