using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAdditions.Dtos
{
    public class CreateOrEditItemAdditionDto : EntityDto<long?>
    {
        public virtual string Name{ get; set; }
        public virtual string NameEnglish { get; set; }

        public virtual decimal? Price { get; set; }

        public virtual long? ItemId { get; set; }
        public virtual string SKU { get; set; }
        public int MenuType { get; set; }

        public int LanguageBotId { get; set; }

        public virtual long? ItemAdditionsCategoryId { get; set; }
        public virtual bool IsCondiments { get; set; }
        public bool IsDeserts { get; set; }
        public bool IsCrispy { get; set; }
        public string ImageUri { get; set; }


  
        public decimal LoyaltyPoints { get; set; }
        public decimal OriginalLoyaltyPoints { get; set; }
        public bool IsOverrideLoyaltyPoints { get; set; }

    }
}
