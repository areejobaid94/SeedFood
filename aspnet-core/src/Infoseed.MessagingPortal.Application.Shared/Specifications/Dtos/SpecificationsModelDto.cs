using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Specifications.Dtos
{
    public class SpecificationsModelDto
    {

        public SpecificationsDto specificationsDto { get; set; }
        public List<SpecificationChoicesDto> specificationChoicesDtos { get; set; }
        public long LoyaltyDefinitionId { get; set; }

        public long CreatedBy { get; set; }
    }
}
