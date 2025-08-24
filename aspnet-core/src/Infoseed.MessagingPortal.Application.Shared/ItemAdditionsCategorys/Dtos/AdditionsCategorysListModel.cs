using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos
{
    public class AdditionsCategorysListModel
    {

        public int AdditionsAndItemId { get; set; }
        public int Id { get; set; }
        public virtual long ItemAdditionsId { get; set; }
        public string Name { get; set; }

        public string NameEnglish { get; set; }
        public List<ItemAdditionDto> ItemAdditionDto { get; set; }
        public bool IsCondiments { get; set; }
        public bool IsDeserts { get; set; }
        public bool IsCrispy { get; set; }
    }
}
