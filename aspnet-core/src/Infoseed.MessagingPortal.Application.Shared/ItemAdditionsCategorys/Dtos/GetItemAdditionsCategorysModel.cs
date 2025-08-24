using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos
{
    public class GetItemAdditionsCategorysModel
    {

        public int categoryPriority { get; set; }

        public long categoryId { get; set; }
        public string categoryName { get; set; }
        public string categoryNameEnglish { get; set; }
        public bool IsCondiments { get; set; }
        public bool IsDeserts { get; set; }
        public bool IsCrispy { get; set; }
        public bool IsNon { get; set; }
        public List<ItemAdditionDto> listItemAdditionsCategories { get; set; }
    }
}
