
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.OrderLineAdditionalIngredients.Dtos
{
    public class OrderLineAdditionalIngredientDto : EntityDto<long>
    {
		public string Remarks { get; set; }

		public decimal? Total { get; set; }

		public int? Quantity { get; set; }

		public decimal? UnitPrice { get; set; }


		 public long? OrderId { get; set; }

		 
    }
}