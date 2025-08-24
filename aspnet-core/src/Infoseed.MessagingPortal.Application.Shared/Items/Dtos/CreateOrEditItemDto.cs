
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using System.Collections.Generic;
using Infoseed.MessagingPortal.ItemAndAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.Specifications.Dtos;

namespace Infoseed.MessagingPortal.Items.Dtos
{
    public class CreateOrEditItemDto : EntityDto<long?>
    {


        public int? Qty { get; set; }
        public string Size { get; set; }

        public int TenantId { get; set; }

        public decimal? OldPrice { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }


        public long MenuId { get; set; }

        public long? CreatorUserId { get; set; }
		public string Ingredients { get; set; }
		
		
		[Required]
		[StringLength(ItemConsts.MaxItemNameLength, MinimumLength = ItemConsts.MinItemNameLength)]
		public  string ItemName { get; set; }
		public  string ItemDescription { get; set; }


		public  string ItemNameEnglish { get; set; }
		public  string ItemDescriptionEnglish { get; set; }

		public  string CategoryNames { get; set; }

		public  string CategoryNamesEnglish { get; set; }



		public bool IsInService { get; set; }


        public int? Status_Code { get; set; }

        [Required]
		public DateTime CreationTime { get; set; }
		
		
		public DateTime? DeletionTime { get; set; }
		
		
		public DateTime? LastModificationTime { get; set; }

		public virtual decimal? Price { get; set; }

		public virtual string ImageUri { get; set; }

		public int Priority { get; set; }

		public virtual long? ItemCategoryId { get; set; }
		public virtual long? ItemSubCategoryId { get; set; }
		public virtual string SKU { get; set; }

		public ItemAdditionDto[] itemAdditionDtos { get; set; }


		public int MenuType { get; set; }
		public int LanguageBotId { get; set; }


		public virtual string Barcode { get; set; }

		public List<CreateOrEditItemAndAdditionsCategoryDto> lstItemAndAdditionsCategoryDto { get; set; }
		public List<ItemSpecificationsDto> lstItemSpecificationsDto { get; set; }
		public List<ItemImagesModel> lstItemImages { get; set; }
		public string AreaIds { get; set; }
		public bool IsQuantitative { get; set; }

		public bool IsLoyal { get; set; }
		public decimal LoyaltyPoints { get; set; }
		public decimal OriginalLoyaltyPoints { get; set; }
		public bool IsOverrideLoyaltyPoints { get; set; }
		public long? LoyaltyDefinitionId { get; set; }



        public string InServiceIds { get; set; }
		public decimal ItemDiscount { get; set; }

    }
}