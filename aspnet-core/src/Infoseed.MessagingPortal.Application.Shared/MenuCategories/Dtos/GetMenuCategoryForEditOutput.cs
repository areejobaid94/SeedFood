using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.MenuCategories.Dtos
{
    public class GetMenuCategoryForEditOutput
    {
		public CreateOrEditMenuCategoryDto MenuCategory { get; set; }


    }
}