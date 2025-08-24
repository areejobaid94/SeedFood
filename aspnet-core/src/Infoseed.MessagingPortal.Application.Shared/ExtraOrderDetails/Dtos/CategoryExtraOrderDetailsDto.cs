using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ExtraOrderDetails.Dtos
{
    public class CategoryExtraOrderDetailsDto
    {
        public virtual string SpecificationName { get; set; }
        public virtual string SpecificationNameEnglish { get; set; }
        public virtual int SpecificationUniqueId { get; set; }

        public List<ExtraOrderDetailsDto> lstExtraOrderDetailsDto { get; set; }
    }
}
