using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.OrderLineAdditionalIngredients.Dtos
{
    public class GetOrderLineAdditionalIngredientForEditOutput
    {
		public CreateOrEditOrderLineAdditionalIngredientDto OrderLineAdditionalIngredient { get; set; }

		public string OrderOrderRemarks { get; set;}


    }
}