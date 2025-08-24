using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace BotService.Models.API
{
    public class Item
    {
        public long Id { get; set; }
        public Item()
        {
            ItemSpecifications = new List<ItemSpecification>();

        }
        public long? CreatorUserId { get; set; }
        public int? Qty { get; set; }
        public string Ingredients { get; set; }

        public string ItemName { get; set; }
        public string ItemDescription { get; set; }


        public string ItemNameEnglish { get; set; }
        public string ItemDescriptionEnglish { get; set; }

        public string CategoryNames { get; set; }

        public string CategoryNamesEnglish { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryNameEnglish { get; set; }

        public bool IsInService { get; set; }
        public bool IsDeleted { get; set; }



        public DateTime CreationTime { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public string ImageUri { get; set; }
        public decimal? Price { get; set; }
        public decimal? ViewPrice { get; set; }
        public int Priority { get; set; }

        public virtual string SKU { get; set; }
        public long ItemCategoryId { get; set; }
        public long ItemSubCategoryId { get; set; }

        public string Size { get; set; }

        public int TenantId { get; set; }

        public decimal? OldPrice { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }


        public long MenuId { get; set; }

        public ItemAddition[] itemAdditionDtos { get; set; }



        public int MenuType { get; set; }
        public int LanguageBotId { get; set; }
        public List<ItemSpecification> ItemSpecifications { get; set; }

        public List<AdditionsCategorysListModel> additionsCategorysListModels { get; set; }


        public string Barcode { get; set; }


        public string Discount { get; set; }
        public string DiscountImg { get; set; }

        public int? Status_Code { get; set; }
        public DateTime? LastModifierDateC { get; set; }
        public List<CreateOrEditItemAndAdditionsCategory> lstItemAndAdditionsCategoryDto { get; set; }
        public List<ItemSpecifications> lstItemSpecificationsDto { get; set; }

        public string AreaIds { get; set; }
        public bool IsQuantitative { get; set; }


        public bool HasSpecifications { get; set; }
        public bool HasAdditions { get; set; }



        public bool IsLoyal { get; set; }
        public decimal LoyaltyPoints { get; set; }
        public decimal OriginalLoyaltyPoints { get; set; }
        public bool IsOverrideLoyaltyPoints { get; set; }
        
        public long LoyaltyDefinitionId { get; set; }
    }
}
