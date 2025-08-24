
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.OrderLineAdditionalIngredients.Dtos
{
    public class CreateOrEditOrderLineAdditionalIngredientDto : EntityDto<long?>
    {

		public string Remarks { get; set; }
		
		
		public decimal? Total { get; set; }
		
		
		public int? Quantity { get; set; }
		
		
		public decimal? UnitPrice { get; set; }
		
		
		 public long? OrderId { get; set; }
		 
		 
    }
}