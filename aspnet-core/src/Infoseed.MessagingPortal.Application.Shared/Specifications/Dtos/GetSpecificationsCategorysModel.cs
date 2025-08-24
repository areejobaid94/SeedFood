using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Specifications.Dtos
{
    public class GetSpecificationsCategorysModel
    {
        public int categoryPriority { get; set; }

        public long categoryId { get; set; }
        public string categoryName { get; set; }
        public string categoryNameEnglish { get; set; }

        public int itemSpecificationId { get; set; }
        public bool IsMultipleSelection { get; set; }
        public bool IsRequired { get; set; }
        public int MaxSelectNumber { get; set; }
        public List<SpecificationChoicesDto> listSpecificationChoices { get; set; }
    }
}
