using System.Collections.Generic;

namespace BotService.Models.API
{
    public class CategoryExtraOrderDetails
    {
        public virtual string SpecificationName { get; set; }
        public virtual string SpecificationNameEnglish { get; set; }
        public virtual int SpecificationUniqueId { get; set; }

        public List<ExtraOrderDetails> lstExtraOrderDetailsDto { get; set; }
    }
}
