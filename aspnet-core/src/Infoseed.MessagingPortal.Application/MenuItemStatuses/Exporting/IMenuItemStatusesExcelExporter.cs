using System.Collections.Generic;
using Infoseed.MessagingPortal.MenuItemStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.MenuItemStatuses.Exporting
{
    public interface IMenuItemStatusesExcelExporter
    {
        FileDto ExportToFile(List<GetMenuItemStatusForViewDto> menuItemStatuses);
    }
}