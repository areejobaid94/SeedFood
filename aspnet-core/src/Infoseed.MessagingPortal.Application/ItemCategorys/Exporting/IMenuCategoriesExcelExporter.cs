using System.Collections.Generic;
using Infoseed.MessagingPortal.MenuCategories.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.MenuCategories.Exporting
{
    public interface IMenuCategoriesExcelExporter
    {
        FileDto ExportToFile(List<GetMenuCategoryForViewDto> menuCategories);
    }
}